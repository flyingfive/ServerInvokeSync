using MsgPack.Serialization;
//using Newtonsoft.Json;
using SSock.Message;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace SSock.Server
{
    /// <summary>
    /// 接收到消息包事件参数
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 消息是否已被处理
        /// </summary>
        public bool Processed { get; set; }
        /// <summary>
        /// 收到的消息
        /// </summary>
        public MessageDataPacket ReceivedMessage { get; private set; }

        public MessageReceivedEventArgs(MessageDataPacket messageDataPacket)
        {
            ReceivedMessage = messageDataPacket;
        }
    }

    /// <summary>
    /// 远程调用消息事件参数
    /// </summary>
    public class InvokeMessageEventArgs : EventArgs
    {
        /// <summary>
        /// 接收到的远程调用方法
        /// </summary>
        public MessageDataPacket InvokeMessage { get; private set; }
        /// <summary>
        /// 需要返回（回复）的数据
        /// </summary>
        public object ReturnData { get; set; }

        public InvokeMessageEventArgs(MessageDataPacket dataPacket) { InvokeMessage = dataPacket; }
    }

    public class SockServer : AppServer<SockSession, BinaryRequestInfo>
    {
        /// <summary>
        /// 一个完整的消息收到后触发
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
        /// <summary>
        /// 处理向客户端的远程调用发起后，收到客户端的调用返回信息时发生
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> OnClientReturned;
        /// <summary>
        /// 远程调用时发生，处理客户端主动发起的远程调用请求
        /// </summary>
        public event EventHandler<InvokeMessageEventArgs> OnClientInvoking;

        /// <summary>
        /// 服务启动后
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// 服务关闭后
        /// </summary>
        public event EventHandler Stopped;

        private IJsonSerialization _jsonHelper = null;

        internal SockServer() : base(new FixedHeadPacketReceiveFilterFactory())
        {
            _jsonHelper = JsonUtils.GetSerializer();
            this.NewRequestReceived += SockServer_NewRequestReceived;
        }

        private SockConfig _sockConfig = null;

        public void Setup(SockConfig sockConfig)
        {
            _sockConfig = sockConfig;
            var config = new ServerConfig()
            {
                Port = sockConfig.Port,
                //可允许连接的最大连接数;
                MaxConnectionNumber = sockConfig.MaxConnectionNumber,
                //最大允许的请求长度，默认值为1024;
                MaxRequestLength = sockConfig.MaxRequestLength,
                ReceiveBufferSize = sockConfig.BufferSize,
                SendBufferSize = sockConfig.BufferSize,
                Name = sockConfig.ServerName,
                //文本的默认编码，默认值是 ASCII;
                //TextEncoding = "UTF-8",
                LogAllSocketException = true,
                //网络连接正常情况下的keep alive数据的发送间隔, 默认值为 600, 单位为秒;
                KeepAliveTime = sockConfig.KeepAliveTime,
                //Keep alive失败之后, keep alive探测包的发送间隔，默认值为 60, 单位为秒;
                KeepAliveInterval = 60,
                // true 或 false, 是否定时清空空闲会话，默认值是 false;
                ClearIdleSession = false,
                //监听队列的大小;
                ListenBacklog = 500,
            };
            base.Setup(config);
        }

        private void SockServer_NewRequestReceived(SockSession session, BinaryRequestInfo requestInfo)
        {
            var bufferData = requestInfo.Body;
            var serializer = SerializationContext.Default.GetSerializer<MessageDataPacket>();
            using (var stream = new MemoryStream(bufferData))
            {
                var message = serializer.Unpack(stream);
                Array.Clear(bufferData, 0, bufferData.Length);
                if (message.MessageType == MessageType.Identity)
                {
                    UpdateIdentity(session, message);
                    return;
                }
                var eventArgs = new MessageReceivedEventArgs(message);
                //seq1:先处理调用的返回消息
                if (message.MessageType == MessageType.Return)
                {
                    if (session.CurrentChannel == null || session.CurrentChannel.HasTimeout) { return; }            //超时抛弃
                    if (this.OnClientReturned != null)
                    {
                        //事件通知定向发送：这种方式调用可减少外部事件的冗余处理，仅针对正在处理指定远程调用消息ID的SSockChannel实例发送接收到返回数据的事件通知
                        var target = this.OnClientReturned.GetInvocationList()
                            .Select(d => new { target = d.Target as SSockChannel, method = d })
                            //所有SSockChannel类型的订阅者
                            .Where(d => d.target != null)
                            //只关心此条回复消息的订阅者（处理其它远程调用请求的订阅者不传递此条回复事件）
                            .Where(d => d.target.RemoteInvokingMessageId == message.ReferId)
                            .FirstOrDefault();
                        if (target != null && target.method != null)
                        {
                            target.method.DynamicInvoke(new object[] { session, eventArgs });
                        }
                        else
                        {
                            //定向通知找不到订阅者的情况下广播发送此事件
                            //事件通知广播传递：这种简单方式的事件调用（所有有效的订阅者都会触发事件方法），在外部上一次的通讯信道还没有及时Dispose前，会产生冗余的事件触发
                            this.OnClientReturned(session, eventArgs);
                        }
                    }
                    return;
                }
                //seq2:处理客户端的主动调用消息
                if (message.MessageType == MessageType.Invoke)
                {
                    var args = new InvokeMessageEventArgs(message);
                    if (OnClientInvoking != null)
                    {
                        OnClientInvoking(session, args);
                    }
                    else
                    {
                        args.ReturnData = HandleClientInvoke(message, session);
                    }
                    var json = string.Empty;
                    if (args.ReturnData != null)
                    {
                        json = _jsonHelper.Serialize(args.ReturnData);
                    }
                    var returnPacket = new MessageDataPacket() { ClientId = message.ClientId, MessageBody = json, MessageType = MessageType.Return, ReferId = message.Id };
                    session.Send(returnPacket);
                    //Array.Clear(bufferData, 0, bufferData.Length);
                    return;
                }
                //seq3:最后由外部处理消息(PS:消息的Processed字段为true表示消息已被处理过)
                var messageReceived = this.OnMessageReceived;
                if (messageReceived != null)
                {
                    messageReceived(session, eventArgs);
                }
            }
            //Array.Clear(bufferData, 0, bufferData.Length);
        }

        /// <summary>
        /// 子类自定义实现处理客户端的远程调用请求
        /// </summary>
        /// <param name="message"></param>
        /// <param name="clientSession"></param>
        protected object HandleClientInvoke(MessageDataPacket message, SockSession clientSession)
        {
            throw new NotImplementedException("请在子类中重写此方法已完成客户端的远程调用请求!");
        }

        /// <summary>
        /// 更新客户端业务标识(Identity消息类型接收后处理)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        protected void UpdateIdentity(SockSession session, MessageDataPacket message)
        {
            var clientId = message.ClientId;
            if (string.IsNullOrWhiteSpace(clientId))
            {
                session.Close(CloseReason.ServerClosing);
                return;
            }
            var hostName = message.MessageBody;
            var connectedSession = this.GetSessionByClientId(clientId);
            //同一个客户端第二次连接时踢除上一个会话
            if (string.Equals(_sockConfig.AbandonDuplicateClient, "Previous", StringComparison.CurrentCultureIgnoreCase))
            {
                if (connectedSession != null)
                {
                    //remark:同一业务标识已经有存在的一次连接
                    connectedSession.Send(new MessageDataPacket() { ClientId = clientId, MessageType = MessageType.Notice, Action = FixedFlags.ABANDON_CONN });
                }
            }
            //同一个客户端第二次连接时保持上一个会话，踢除当前会话
            if (string.Equals(_sockConfig.AbandonDuplicateClient, "Current", StringComparison.CurrentCultureIgnoreCase))
            {
                session.Send(new MessageDataPacket() { ClientId = clientId, MessageType = MessageType.Notice, Action = FixedFlags.ABANDON_CONN });
                return;
            }
            session.ClientID = clientId;
            session.HostName = hostName;
            this.OnClientIdentified?.Invoke(session, new ClientSocketEventArgs(new SocketClientInfo() { ClientId = session.ClientID, SessionId = session.SessionID, LastActiveTime = session.LastActiveTime, StartTime = session.StartTime, RemoteAddress = session.RemoteEndPoint.ToString(), HostName = session.HostName }));
        }

        /// <summary>
        /// 识别到Socket会话客户端的业务标识后发生
        /// </summary>
        public event EventHandler<ClientSocketEventArgs> OnClientIdentified;
        public event EventHandler<ClientClosedEventArgs> OnClientClosed;

        protected override void OnNewSessionConnected(SockSession session)
        {
            base.OnNewSessionConnected(session);
        }


        protected override void OnSessionClosed(SockSession session, CloseReason reason)
        {
            if (reason == CloseReason.SocketError)
            {
                Logger.Error(string.Format("SocketError: sessionID:{0}, ClientID:{1}, RemoteAddress:{2}, LastActiveTime:{3}, StartTime:{4}, KeepAliveTime:{5}, TotalHandledRequests:{6}"
                    , session.SessionID
                    , session.ClientID
                    , session.RemoteEndPoint.ToString()
                    , session.LastActiveTime.ToString("yyyy-MM-dd HH:mm:ss")
                    , session.StartTime.ToString("yyyy-MM-dd HH:mm:ss")
                    , this.Config.KeepAliveTime.ToString()
                    , base.TotalHandledRequests.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(session.ClientID))
            {
                this.OnClientClosed?.Invoke(session, new ClientClosedEventArgs(reason, new SocketClientInfo() { ClientId = session.ClientID, SessionId = session.SessionID, LastActiveTime = session.LastActiveTime, StartTime = session.StartTime, RemoteAddress = session.RemoteEndPoint.ToString(), HostName = session.HostName }));
            }
            base.OnSessionClosed(session, reason);
        }

        /// <summary>
        /// 根据外部业务标识clientId获取客户端连接
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public SockSession GetSessionByClientId(string clientId)
        {
            var session = base.GetSessions((s) => { return string.Equals(s.ClientID, clientId); }).FirstOrDefault();
            return session;
        }

        /// <summary>
        /// 检查clientId标识的业务客户端是否在线。
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public bool CheckClientIsOnline(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) { return false; }
            var session = GetSessionByClientId(clientId);
            return session != null && session.Connected;
        }

        protected override void OnStarted()
        {
            base.OnStarted();
            Started?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }

}
