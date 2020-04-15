using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlyingSocket;
using FlyingSocket.Data;
using SuperSocket.SocketBase;

namespace FlyingServer
{
    public class FlyingServerInvokeChannel : ISocketInvokingChannel, IDisposable
    {
        public bool HasTimeout { get; private set; }

        public string ClientId { get; private set; }

        public event EventHandler<EventArgs> Timeout;

        private FlyingSocketServer _flyingSocketServer = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="serverName"></param>
        public FlyingServerInvokeChannel(string clientId, string serverName)
        {
            if (string.IsNullOrWhiteSpace(clientId)) { throw new ArgumentException(); }
            if (string.IsNullOrWhiteSpace(serverName)) { throw new ArgumentException(); }
            this.ClientId = clientId;
            var server = FlyingServerContainer.GetServerByName(serverName);
            if (server == null) { throw new InvalidOperationException(); }
            _flyingSocketServer = server;
        }


        private Task _commTask = null;                              //异步通讯任务
        private CancellationTokenSource _taskCancellation = null;   //通讯任务线程的取消标记
        private int _timeout = 0;                                   //通讯超时时间，单位毫秒
        private EventWaitHandle _resetEvent = null;
        private FlyingSocketPacket _invokeSendPacket = null;
        private FlyingSocketPacket _invokeReceivePacket = null;
        private FlyingSocketSession _socketSession = null;
        /// <summary>
        /// 当前正在处理的远程调用消息ID
        /// </summary>
        public Guid InvokingPacketId { get { return _invokeSendPacket == null ? Guid.Empty : _invokeSendPacket.Id; } }

        public byte[] Invoke(int timeout, FlyingSocketPacket invokeData)
        {
            if (invokeData == null) { throw new ArgumentNullException(); }
            if (invokeData.PacketType != SocketPacketType.Invoke) { throw new InvalidOperationException("非调用消息不能通过信道处理！"); }

            _timeout = timeout;
            HasTimeout = false;
            _resetEvent = new ManualResetEvent(false);
            var session = _flyingSocketServer.GetSessionByClientId(this.ClientId);
            if (session == null || !session.Connected)
            {
                throw new ApplicationException(string.Format("与客户端：{0}的通讯失败，远程会话不存在或已关闭！", this.ClientId));
            }
            invokeData.ClientId = this.ClientId;
            _invokeSendPacket = invokeData;
            _socketSession = session;
            _flyingSocketServer.EndRemoteInvoking += _flyingSocketServer_EndRemoteInvoking;
            _taskCancellation = new CancellationTokenSource();
            _commTask = new Task(new Action(this.Send), _taskCancellation.Token);
            _commTask.Start();
            var flag = _resetEvent.WaitOne(_timeout);
            _socketSession.EndSyncInvoking();
            HasTimeout = _invokeReceivePacket == null;
            if (HasTimeout)
            {
                if (Timeout != null) { Timeout(this, new SyncSocketChannelTimeoutEventArgs(this._invokeSendPacket) { AbandonedPacket = _invokeReceivePacket }); }
            }
            return HasTimeout ? new byte[0] : _invokeReceivePacket.Body;
        }


        private void Send()
        {
            //保证向同一客户端远程调用的原子操作。客户端会话中，避免当前调用还未接收返回时又发起另一远程调用，会产生callMsg.Id和receiveMsg.ReferId不匹配的错误
            while (_socketSession.IsSyncInvoking)
            {
                Thread.Sleep(100);
            }
            _socketSession.Send(_invokeSendPacket);
        }

        private void _flyingSocketServer_EndRemoteInvoking(object sender, FlyingPacketReceivedEventArgs e)
        {
            if (!string.Equals(e.ReceivedMessage.ClientId, this.ClientId)) { return; }
            if (e.ReceivedMessage.ReferId == _invokeSendPacket.Id)
            {
                _invokeReceivePacket = e.ReceivedMessage;
                e.Processed = true;
                if (!_resetEvent.SafeWaitHandle.IsClosed && !_resetEvent.SafeWaitHandle.IsInvalid) { _resetEvent.Set(); }
            }
            else
            {
                //如果事件发布者采用广播方式传递事件通知，则在冗余的事件处理者中会收到不匹配的回复消息（处理远程调用请求消息ID为abc的实例可能会收到回复123请求的消息）
                //这里忽略不关心的回复消息即可，不用引发异常
                Console.WriteLine(string.Format("server invoke msg Id:{0}, received msg Id:{1}", _invokeSendPacket.Id, e.ReceivedMessage.ReferId));
                //throw new InvalidOperationException(string.Format("远程调用收到了不正确的回复，invoke id:{0}, refer id:{1}, received id:{2}", _callMessage.Id, e.ReceivedMessage.ReferId, e.ReceivedMessage.Id));
            }
        }


        private bool _isDisposed = false;
        /// <summary>
        /// 这里只能释放Invoke方法中的资源。接口的同一代理实例多次调用方法时为同一SocketInvocationHandler处理点，
        /// 如释放构造中的引用会导致第二次调用时信道无法完成
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) { return; }
            _socketSession.EndSyncInvoking();
            HasTimeout = false;
            _invokeSendPacket = null;
            _invokeReceivePacket = null;
            _flyingSocketServer.EndRemoteInvoking -= this._flyingSocketServer_EndRemoteInvoking;
            if (_commTask != null && _taskCancellation != null)
            {
                _taskCancellation.Cancel();
                _commTask.Dispose();
                _taskCancellation.Dispose();
            }
            _socketSession = null;
            _flyingSocketServer = null;
            _resetEvent.Dispose();
            _resetEvent = null;
            _isDisposed = true;
        }
    }

    /// <summary>
    /// Socket客户端事件参数
    /// </summary>
    public class FlyingSocketClientEventArgs : EventArgs
    {
        public SocketClientInfo Client { get; protected set; }

        public FlyingSocketClientEventArgs(SocketClientInfo clientInfo)
        {
            if (clientInfo == null) { throw new ArgumentNullException("参数clientInfo不能为null."); }
            this.Client = clientInfo;
        }
    }

    /// <summary>
    /// Socket客户端关闭事件参数
    /// </summary>
    public class FlyingSocketClientClosedEventArgs : FlyingSocketClientEventArgs
    {
        /// <summary>
        /// 关闭原因
        /// </summary>
        public CloseReason CloseReason { get; private set; }
        public FlyingSocketClientClosedEventArgs(CloseReason reason, SocketClientInfo clientInfo) : base(clientInfo)
        {
            this.CloseReason = reason;
        }
    }
}
