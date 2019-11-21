using CSock.Message;
using MsgPack.Serialization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace CSock
{
    /// <summary>
    /// socket客户端
    /// </summary>
    public class SockClient
    {
        private EasyClient _easyClient = null;

        public event EventHandler OnClosed;
        public event EventHandler OnConnected;
        public event EventHandler<SuperSocket.ClientEngine.ErrorEventArgs> OnError;

        /// <summary>
        /// 一个完整的通知类消息收到后触发
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
        /// <summary>
        /// 在远程调用后，Server端返回消息时发生
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> OnServerReturned;

        /// <summary>
        /// 在Server端远程调用时发生，处理Server端的调用请求
        /// </summary>
        public event EventHandler<InvokeMessageEventArgs> OnServerInvoking;
        /// <summary>
        /// 客户端的外部业务标识
        /// </summary>
        public string ClientID { get; private set; }

        private IPEndPoint _serverAddres = null;        //远程服务器地址

        /// <summary>
        /// 当前调用通讯信道（超时则消失）
        /// </summary>
        private ISockChannel _currentChannel = null;
        /// <summary>
        /// 发送时是否掉线重连
        /// </summary>
        public bool ReconnectOnSend { get; set; }
        /// <summary>
        /// 发送时掉线时否引发异常
        /// </summary>
        public bool OccurExceptionOnSend { get; set; }
        /// <summary>
        /// 是否连接上服务器
        /// </summary>
        public bool Connected { get { return _easyClient.IsConnected; } }

        private IJsonSerialization _jsonHelper = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId">外部业务标识</param>
        public SockClient(string clientId)
        {
            ClientID = clientId;
            _easyClient = new EasyClient();
            _easyClient.Closed += _client_Closed;
            _easyClient.Connected += _client_Connected;
            _easyClient.Error += _client_Error;
            _easyClient.Initialize(new FixedHeadPacketReceiveFilter(), this.HandlePacket);
            _jsonHelper = JsonUtils.GetSerializer();
        }

        private void _client_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            if (OnError != null) { OnError(sender, e); }
        }

        private void _client_Connected(object sender, EventArgs e)
        {
            var packet = new MessageDataPacket() { ClientId = ClientID, MessageBody = Dns.GetHostName(), MessageType = MessageType.Identity };
            Send(packet);
            if (OnConnected != null) { OnConnected(this, e); }
        }

        private void _client_Closed(object sender, EventArgs e)
        {
            _serverAddres = null;
            if (OnClosed != null) { OnClosed(this, e); }
        }

        public bool Connect(IPEndPoint address)
        {
            _serverAddres = address;
            var connected = _easyClient.ConnectAsync(address).Result;
            return connected;
        }

        public bool Close()
        {
            var flag = _easyClient.Close().Result;
            return flag;
        }

        /// <summary>
        /// 标识开始服务端的远程调用
        /// </summary>
        public void BeginRemoteInvoking(ISockChannel sockChannel)
        {
            if (sockChannel == null) { throw new ArgumentNullException("sockChannel不能为NULL!"); }
            Interlocked.Exchange<ISockChannel>(ref _currentChannel, sockChannel);
            Interlocked.Exchange(ref _remote_invoking_flag, 1L);
        }

        /// <summary>
        /// 标识服务端的远程调用结束
        /// </summary>
        public void EndRemoteInvoking()
        {
            if (!IsServerInvoking) { return; }
            Interlocked.Exchange<ISockChannel>(ref _currentChannel, null);
            Interlocked.Exchange(ref _remote_invoking_flag, 0L);
        }

        private void HandlePacket(MessageDataPacket dataPacket)
        {
            if (!string.Equals(dataPacket.ClientId, this.ClientID))        //server端发送对象错误，不是应该由此客户端处理
            {
                return;
            }
            var messageArgs = new MessageReceivedEventArgs(dataPacket);
            if (dataPacket.MessageType == MessageType.Invoke)               //处理server端的调用消息，事件处理或子类重写实现
            {
                var args = new InvokeMessageEventArgs(dataPacket);
                if (OnServerInvoking != null)
                {
                    OnServerInvoking(this, args);
                }
                else
                {
                    args.ReturnData = HandleServerInvoke(dataPacket);
                }
                var json = string.Empty;
                if (args.ReturnData != null)
                {
                    json = _jsonHelper.Serialize(args.ReturnData);//JsonConvert.SerializeObject(args.ReturnData);
                }
                var returnPacket = new MessageDataPacket() { ClientId = ClientID, MessageBody = json, MessageType = MessageType.Return, ReferId = dataPacket.Id };
                Send(returnPacket);
                return;                                                     //返回：不再需要外部的OnMessageReceived来处理
            }
            if (dataPacket.MessageType == MessageType.Notice)               //服务端通知断开连接                
            {
                if (string.Equals(dataPacket.Action, FixedFlags.ABANDON_CONN))
                {
                    var closed = this._easyClient.Close().Result;
                    return;                                                     //返回：不再需要外部的OnMessageReceived来处理
                }
                if (OnMessageReceived != null)
                {
                    OnMessageReceived(this, messageArgs);
                }
            }
            if (dataPacket.MessageType == MessageType.Return)               //处理服务端的返回消息
            {
                if (_currentChannel == null || _currentChannel.HasTimeout)
                {
                    return;         //超时丢弃
                }
                if (OnServerReturned != null)
                {
                    OnServerReturned(this, messageArgs);
                }
                //if (_callMessage != null)
                //{
                //    if (_callMessage.Id != dataPacket.ReferId)
                //    {
                //        throw new InvalidDataException(string.Format("call Id:{0},reply Id:{1}", _callMessage.Id, dataPacket.ReferId));
                //    }
                //    if (OnMessageReturned != null)
                //    {
                //        OnMessageReturned(this, messageArgs);
                //    }
                //    Interlocked.Exchange<MessageDataPacket>(ref _callMessage, null);
                //}
                //else
                //{
                //    throw new InvalidOperationException("no invoke message current.");
                //}
                return;
            }
        }

        protected virtual object HandleServerInvoke(MessageDataPacket dataPacket)
        {
            throw new NotImplementedException("请在子类中重写此方法已完成服务端的远程调用请求!");
            //var routeData = dataPacket.Action.Split(new char[] { ':' });
            //var serviceName = routeData.First();
            //var actionName = routeData.Last();
            //todo:default implementation by reflection
        }

        public void Send(string content)
        {
            var packet = new MessageDataPacket() { ClientId = ClientID, MessageBody = content, MessageType = MessageType.Notice };
            this.Send(packet);
        }

        /// <summary>
        /// 标识此客户端当前是否正处于远程调用服务端的过程中
        /// </summary>
        public bool IsServerInvoking { get { return Interlocked.Read(ref _remote_invoking_flag) > 0; } }

        //远程调用标识，设计为静态表示全局并发控制。这里为实例字段，控制单个SockClient实例对象不能同时并发处理远程的Server调用。必需在一个调用收到回复或超时后再发起第二次远程调用信道
        private long _remote_invoking_flag = 0;

        /// <summary>
        /// 发送一个标准的通讯消息数据包
        /// </summary>
        /// <param name="dataPacket"></param>
        public void Send(MessageDataPacket dataPacket)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = SerializationContext.Default.GetSerializer<MessageDataPacket>();
                serializer.Pack(stream, dataPacket);
                var bodyData = stream.ToArray();
                this.Send(bodyData);
            }
        }

        private void Send(byte[] dataBody)
        {
            if (!_easyClient.IsConnected) { return; }
            var commandData = Encoding.UTF8.GetBytes(FixedFlags.CMD_KEY);//协议命令只占4位,如果占的位数长过协议，那么协议解析肯定会出错的
            var dataLen = BitConverter.GetBytes(dataBody.Length);//int类型占4位，根据协议这里也只能4位，否则会出错
            var sendData = new byte[8 + dataBody.Length];//命令加内容长度为8
            Array.ConstrainedCopy(commandData, 0, sendData, 0, 4);
            Array.ConstrainedCopy(dataLen, 0, sendData, 4, 4);
            Array.ConstrainedCopy(dataBody, 0, sendData, 8, dataBody.Length);
            _easyClient.Send(sendData);
            //Array.Clear(commandData, 0, commandData.Length);            //liubq:发送后清除对内存及时回收有帮助（减少内存开销），但极端高并发场景下，可能会造成服务端接收数据不成功，客户端这里不建议放开Array.Clear及时清理
            //Array.Clear(dataBody, 0, dataBody.Length);
            //Array.Clear(dataLen, 0, dataLen.Length);
            //Array.Clear(sendData, 0, sendData.Length);
        }
    }

}
