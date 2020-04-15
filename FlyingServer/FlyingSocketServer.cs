using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyingServer.ReceiveFilter;
using FlyingSocket;
using FlyingSocket.Data;
using SSock.Server;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace FlyingServer
{
    public class FlyingSocketServer : AppServer<FlyingSocketSession, BinaryRequestInfo>
    {
        public const string DEFAULT_SERVER_NAME = "default_flying_server";

        /// <summary>
        /// 一个完整的消息收到后触发
        /// </summary>
        public event EventHandler<FlyingPacketReceivedEventArgs> OnMessageReceived;
        /// <summary>
        /// 结束向客户端发起的远程同步调用。（服务端主动请求）
        /// </summary>
        public event EventHandler<FlyingPacketReceivedEventArgs> EndRemoteInvoking;
        /// <summary>
        /// 开启处理客户端的远程同步调用，需要即时处理并返回数据。（客户端主动请求）
        /// </summary>
        public event EventHandler<RemoteInvokingEventArgs> BeginRemoteInvoking;
        /// <summary>
        /// 客户端关闭连接时发生
        /// </summary>
        public event EventHandler<FlyingSocketClientClosedEventArgs> ClientClosed;
        ///// <summary>
        ///// 客户端请求身份认证时发生
        ///// </summary>
        //public event EventHandler<FlyingPacketReceivedEventArgs> ClientOpened;

        /// <summary>
        /// 服务启动后
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// 服务关闭后
        /// </summary>
        public event EventHandler Stopped;

        public ServerConfig ServerConfig { get; private set; }

        public static readonly ServerConfig DefaultConfig = new ServerConfig()
        {
            Port = 8098,
            MaxConnectionNumber = 20,
            MaxRequestLength = int.MaxValue,
            ReceiveBufferSize = 1024 * 100,
            SendBufferSize = 1024 * 100,
            Name = DEFAULT_SERVER_NAME,
            //TextEncoding = "UTF-8",
            LogAllSocketException = true,
            KeepAliveTime = 60,
            KeepAliveInterval = 60,
            ClearIdleSession = false,
            ListenBacklog = 50
        };

        public FlyingSocketServer() : base(new FixedHeadPacketReceiveFilterFactory())
        {
            this.NewRequestReceived += FlyingSocketServer_NewRequestReceived;
        }

        private void FlyingSocketServer_NewRequestReceived(FlyingSocketSession session, BinaryRequestInfo requestInfo)
        {
            var bufferData = requestInfo.Body;
            var dataPacket = SerializationUtil.BinaryDeserialize<FlyingSocketPacket>(bufferData);
            Array.Clear(bufferData, 0, bufferData.Length);
            switch (dataPacket.PacketType)
            {
                case SocketPacketType.HeartBeat:
                    break;
                case SocketPacketType.Notice:
                    HandleNoticePacket(session, dataPacket);
                    break;
                case SocketPacketType.Invoke:
                    HandleInvokePacket(session, dataPacket);
                    break;
                case SocketPacketType.Return:
                    HandleReturnPacket(session, dataPacket);
                    break;
                case SocketPacketType.Authentication:
                    HandleAuthenticationPacket(session, dataPacket);
                    break;
            }
        }

        private void HandleAuthenticationPacket(FlyingSocketSession session, FlyingSocketPacket dataPacket)
        {
            var clientId = dataPacket.ClientId;
            if (string.IsNullOrWhiteSpace(clientId))
            {
                Logger.Error(string.Format("关闭{0}重复的连接。", clientId));
                session.Close(CloseReason.ServerClosing);
                return;
            }
            if (CheckClientIsOnline(clientId))
            {
                Logger.Error(string.Format("关闭{0}重复的连接。", clientId));
                session.Close(CloseReason.ServerClosing);
                return;
            }
            //var hostName = dataPacket.MessageBody;
            //var connectedSession = this.GetSessionByClientId(clientId);
            //同一个客户端第二次连接时踢除上一个会话
            //if (string.Equals(_sockConfig.AbandonDuplicateClient, "Previous", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    if (connectedSession != null)
            //    {
            //        //remark:同一业务标识已经有存在的一次连接
            //        connectedSession.Send(new MessageDataPacket() { ClientId = clientId, MessageType = MessageType.Notice, Action = FixedFlags.ABANDON_CONN });
            //    }
            //}
            ////同一个客户端第二次连接时保持上一个会话，踢除当前会话
            //if (string.Equals(_sockConfig.AbandonDuplicateClient, "Current", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    session.Send(new MessageDataPacket() { ClientId = clientId, MessageType = MessageType.Notice, Action = FixedFlags.ABANDON_CONN });
            //    return;
            //}
            //session.ClientID = clientId;
            //session.HostName = hostName;
            //this.OnClientIdentified?.Invoke(session, new ClientSocketEventArgs(new SocketClientInfo() { ClientId = session.ClientID, SessionId = session.SessionID, LastActiveTime = session.LastActiveTime, StartTime = session.StartTime, RemoteAddress = session.RemoteEndPoint.ToString(), HostName = session.HostName }));
        }

        /// <summary>
        /// 处理客户端发起的异步通知数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dataPacket"></param>
        private void HandleNoticePacket(FlyingSocketSession session, FlyingSocketPacket dataPacket)
        {
            var messageReceived = this.OnMessageReceived;
            if (messageReceived != null)
            {
                messageReceived(session, new FlyingPacketReceivedEventArgs(dataPacket));
            }
        }

        /// <summary>
        /// 处理客户端发起的远程同步调用数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dataPacket"></param>
        private void HandleInvokePacket(FlyingSocketSession session, FlyingSocketPacket dataPacket)
        {
            var args = new RemoteInvokingEventArgs(dataPacket);
            if (BeginRemoteInvoking != null)
            {
                BeginRemoteInvoking(session, args);
            }
            else
            {
                args.ReturnData = HandleClientInvoke(session, dataPacket);
            }
            var bodyData = new byte[0];
            if (args.ReturnData != null)
            {
                bodyData = SerializationUtil.BinarySerialize(args.ReturnData);
            }
            var returnPacket = new FlyingSocketPacket() { ClientId = session.ClientId, Body = bodyData, PacketType = SocketPacketType.Return, ReferId = dataPacket.Id };
            session.Send(returnPacket);
            //Array.Clear(bufferData, 0, bufferData.Length);
            return;

        }


        /// <summary>
        /// 子类自定义实现处理客户端的远程调用请求
        /// </summary>
        /// <param name="message"></param>
        /// <param name="clientSession"></param>
        protected virtual object HandleClientInvoke(FlyingSocketSession clientSession, FlyingSocketPacket dataPacket)
        {
            throw new NotImplementedException("请在子类中重写此方法已完成客户端的远程调用请求!");
        }

        /// <summary>
        /// 处理远程同步调用后客户端返回的数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        protected void HandleReturnPacket(FlyingSocketSession session, FlyingSocketPacket packet)
        {
            if (session.CurrentChannel == null) { return; }
            if (session.CurrentChannel.HasTimeout) { return; }
            var eventArgs = new FlyingPacketReceivedEventArgs(packet);
            if (EndRemoteInvoking != null)
            {
                //事件通知定向发送：这种方式调用可减少外部事件的冗余处理，仅针对正在处理指定远程调用消息ID的SSockChannel实例发送接收到返回数据的事件通知
                var target = this.EndRemoteInvoking.GetInvocationList()
                    .Select(d => new { target = d.Target as FlyingServerInvokeChannel, method = d })
                    //所有SSockChannel类型的订阅者
                    .Where(d => d.target != null)
                    //只关心此条回复消息的订阅者（处理其它远程调用请求的订阅者不传递此条回复事件）
                    .Where(d => d.target.InvokingPacketId == packet.ReferId)
                    .FirstOrDefault();
                if (target != null && target.method != null)
                {
                    target.method.DynamicInvoke(new object[] { session, eventArgs });
                }
                else
                {
                    //todo:是否作为异常处理。。。。？
                    //定向通知找不到订阅者的情况下广播发送此事件
                    //事件通知广播传递：这种简单方式的事件调用（所有有效的订阅者都会触发事件方法），在外部上一次的通讯信道还没有及时Dispose前，会产生冗余的事件触发
                    this.EndRemoteInvoking(session, eventArgs);
                }
            }
        }

        protected override void OnNewSessionConnected(FlyingSocketSession session)
        {
            //this.NewSessionConnected
            base.OnNewSessionConnected(session);
        }

        protected override void OnSessionClosed(FlyingSocketSession session, CloseReason reason)
        {
            if (reason == CloseReason.SocketError)
            {
                Logger.Error(string.Format("SocketError: sessionID:{0}, ClientID:{1}, RemoteAddress:{2}, LastActiveTime:{3}, StartTime:{4}, KeepAliveTime:{5}"
                    , session.SessionID
                    , session.ClientId
                    , session.RemoteEndPoint.ToString()
                    , session.LastActiveTime.ToString("yyyy-MM-dd HH:mm:ss")
                    , session.StartTime.ToString("yyyy-MM-dd HH:mm:ss")
                    , this.Config.KeepAliveTime.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(session.ClientId))
            {
                this.ClientClosed?.Invoke(session, new FlyingSocketClientClosedEventArgs(reason, new SocketClientInfo() { ClientId = session.ClientId, SessionId = session.SessionID, LastActiveTime = session.LastActiveTime, StartTime = session.StartTime, RemoteAddress = session.RemoteEndPoint.ToString(), HostName = session.HostName }));
            }
            base.OnSessionClosed(session, reason);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverConfig"></param>
        public void Initialize(ServerConfig serverConfig)
        {
            if (serverConfig == null) { throw new ArgumentNullException(); }
            if (string.IsNullOrWhiteSpace(serverConfig.Name))
            {
                serverConfig.Name = "flying_server_" + Guid.NewGuid().ToString().Split(new char[] { '-' }).First();
            }
            this.ServerConfig = serverConfig;
            base.Setup(serverConfig);
            FlyingServerContainer.AddServer(this);
        }

        /// <summary>
        /// 根据外部业务标识clientId获取客户端连接
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public FlyingSocketSession GetSessionByClientId(string clientId)
        {
            var session = base.GetSessions((s) => { return string.Equals(s.ClientId, clientId); }).FirstOrDefault();
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

    public static class FlyingServerContainer
    {

        private static Dictionary<string, FlyingSocketServer> _servers = null;
        private static object _locker = new object();

        static FlyingServerContainer()
        {
            _servers = new Dictionary<string, FlyingSocketServer>();
        }

        public static FlyingSocketServer GetServerByName(string serverName)
        {
            if (_servers.ContainsKey(serverName))
            {
                lock (_locker)
                {
                    if (_servers.ContainsKey(serverName))
                    {
                        return _servers[serverName];
                    }
                    return null;
                }
            }
            return null;
        }

        internal static void AddServer(FlyingSocketServer server)
        {
            if (server == null) { return; }
            lock (_locker)
            {
                if (_servers.ContainsKey(server.Name))
                {
                    return;
                }
                _servers.Add(server.Name, server);
            }
        }

        ///// <summary>
        ///// 使用程序配置文件构建socket服务
        ///// </summary>
        ///// <returns></returns>
        //public static FlyingSocketServer Build()
        //{
        //    var param = SockConfig.GetConfig();
        //    if (!_servers.ContainsKey(param.ServerName))
        //    {
        //        lock (_locker)
        //        {
        //            if (!_servers.ContainsKey(param.ServerName))
        //            {
        //                var server = new FlyingSocketServer();
        //                server.Setup(param);
        //                _servers.Add(param.ServerName, server);
        //                return server;
        //            }
        //            throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));
        //        }
        //    }
        //    throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));
        //}

        ///// <summary>
        ///// 使用指定参数配置构建socket服务
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public static FlyingSocketServer Build(SockConfig param)
        //{
        //    if (!_servers.ContainsKey(param.ServerName))
        //    {
        //        lock (_locker)
        //        {
        //            if (!_servers.ContainsKey(param.ServerName))
        //            {
        //                var server = new SockServer();
        //                server.Setup(param);
        //                _servers.Add(param.ServerName, server);
        //                return server;
        //            }
        //            throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));
        //        }
        //    }
        //    throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));

        //}
    }
}
