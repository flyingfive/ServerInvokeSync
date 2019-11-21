using SSock.Message;
using SSock.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSock
{
    /// <summary>
    /// 服务端到客户端的远程调用通讯信道
    /// </summary>
    public class SSockChannel : ISockChannel, IDisposable
    {
        public event EventHandler<ChannelTimeoutEventArgs> OnTimeout;
        private string _remoteClientId = string.Empty;
        private SockServer _sockServer = null;

        private Task _commTask = null;                              //异步通讯任务
        private CancellationTokenSource _taskCancellation = null;   //通讯任务线程的取消标记
        private int _timeout = 0;                                   //通讯超时时间，单位毫秒
        private string _remoteResponseData = string.Empty;          //远程客户端响应结果
        private EventWaitHandle _resetEvent = null;
        private SockSession _clientSession = null;
        private MessageDataPacket _callMessage = null;

        //public string ClientSessionId { get { return _clientSession == null ? string.Empty : _clientSession.SessionID; } }
        //public string RemoteClientId { get { return _remoteClientId; } }

        /// <summary>
        /// 当前正在处理的远程调用消息ID
        /// </summary>
        public Guid RemoteInvokingMessageId { get { return _callMessage == null ? Guid.Empty : _callMessage.Id; } }

        /// <summary>
        /// 信道通讯中是否已超时
        /// </summary>
        public bool HasTimeout { get; private set; }

        /// <summary>
        /// 实例化一个服务端远程调用客户端的通讯信道
        /// </summary>
        /// <param name="clientId">远程调用客户端的外部标识ID</param>
        public SSockChannel(string clientId)
        {
            _remoteClientId = clientId;
            var config = SockConfig.GetConfig();
            var server = SockServerFactory.GetServerByName(config.ServerName);
            if (server == null) { throw new InvalidOperationException(string.Format("创建到客户端：{0}的信道失败。系统中不存在名称为：{1}的SockServer", clientId, config.ServerName)); }
            _sockServer = server;
        }

        /// <summary>
        /// 处理信道中接收到客户端的返回消息包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sockServer_OnClientReturned(object sender, MessageReceivedEventArgs e)
        {
            if (!string.Equals(e.ReceivedMessage.ClientId, _remoteClientId)) { return; }
            if (e.ReceivedMessage.ReferId == _callMessage.Id)
            {
                //超时条件已在SockServer中处理
                //if ((DateTime.Now.Subtract(_callMessage.CreateTime).Milliseconds > _timeout)) { HasTimeout = true; return; }            //超时抛弃
                _remoteResponseData = e.ReceivedMessage.MessageBody;
                e.Processed = true;
                if (!_resetEvent.SafeWaitHandle.IsClosed && !_resetEvent.SafeWaitHandle.IsInvalid) { _resetEvent.Set(); }
            }
            else
            {
                //如果事件发布者采用广播方式传递事件通知，则在冗余的事件处理者中会收到不匹配的回复消息（处理远程调用请求消息ID为abc的实例可能会收到回复123请求的消息）
                //这里忽略不关心的回复消息即可，不用引发异常
                Console.WriteLine(string.Format("server invoke msg Id:{0}, received msg Id:{1}", _callMessage.Id, e.ReceivedMessage.ReferId));
                //throw new InvalidOperationException(string.Format("远程调用收到了不正确的回复，invoke id:{0}, refer id:{1}, received id:{2}", _callMessage.Id, e.ReceivedMessage.ReferId, e.ReceivedMessage.Id));
            }
        }

        public string Invoke(int timeout, MessageDataPacket message)
        {
            if (message.MessageType != MessageType.Invoke)
            {
                throw new InvalidOperationException("非调用消息不能通过信道处理！");
            }
            _timeout = timeout;
            HasTimeout = false;
            _resetEvent = new ManualResetEvent(false);
            var session = _sockServer.GetSessionByClientId(_remoteClientId);
            if (session == null || !session.Connected)
            {
                throw new ApplicationException(string.Format("与客户端：{0}的通讯失败，远程会话不存在或已关闭！", _remoteClientId));
            }

            _sockServer.OnClientReturned += _sockServer_OnClientReturned;
            _callMessage = message;
            _clientSession = session;
            message.ClientId = _remoteClientId;
            _taskCancellation = new CancellationTokenSource();
            _commTask = new Task(new Action<object>(this.Send), message, _taskCancellation.Token);
            _commTask.Start();
            var flag = _resetEvent.WaitOne(_timeout);
            _clientSession.EndRemoteInvoking();
            HasTimeout = string.IsNullOrWhiteSpace(_remoteResponseData);
            if (HasTimeout)
            {
                if (OnTimeout != null) { OnTimeout(this, new ChannelTimeoutEventArgs(_callMessage) { ReturnMessage = message }); }
            }
            return _remoteResponseData;
        }

        private void Send(object obj)
        {
            var dataPacket = obj as MessageDataPacket;
            //保证向同一客户端远程调用的原子操作。客户端会话中，当前调用还未接收返回时又发起另一远程调用，会产生callMsg.Id和receiveMsg.ReferId不匹配的错误
            while (_clientSession.IsClientInvoking)
            {
                Thread.Sleep(100);
            }
            _clientSession.BeginRemoteInvoking(this);
            _clientSession.Send(dataPacket);
        }

        private bool _isDisposed = false;
        /// <summary>
        /// 这里只能释放Invoke方法中的资源。接口的同一代理实例多次调用方法时为同一SocketInvocationHandler处理点，
        /// 如释放构造中的引用会导致第二次调用时信道无法完成
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) { return; }
            HasTimeout = false;
            _callMessage = null;
            _remoteResponseData = null;
            _sockServer.OnClientReturned -= _sockServer_OnClientReturned;
            if (_commTask != null && _taskCancellation!=null)
            {
                //while (_commTask.Status != TaskStatus.RanToCompletion)
                //{
                //    Thread.Sleep(300);
                //}
                _taskCancellation.Cancel();
                _commTask.Dispose();
                _taskCancellation.Dispose();
            }
            _resetEvent.Dispose();
            _resetEvent = null;
            _isDisposed = true;

            //HasTimeout = false;
            //_sockServer.OnMessageReceived -= this._sockServer_OnClientReturned;
            //if (_commTask != null) { _commTask.Dispose(); }
            //_resetEvent.Dispose();
            //_remoteResponseData = null;
        }
    }
}
