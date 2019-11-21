using CSock.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSock
{
    /// <summary>
    /// 客户端远程调用服务端方法的通讯信道
    /// </summary>
    public class CSockChannel : ISockChannel
    {
        public event EventHandler<ChannelTimeoutEventArgs> OnTimeout;
        private int _timeout = 0;   //超时值，单位毫秒
        /// <summary>
        /// 信道通讯过程中是否发生超时
        /// </summary>
        public bool HasTimeout { get; private set; }
        private Task _commTask = null;                              //异步通讯任务
        private CancellationTokenSource _taskCancellation = null;   //通讯任务线程的取消标记
        private string _remoteResponseData = string.Empty;          //远程服务端响应数据
        private ManualResetEvent _resetEvent = null;
        private MessageDataPacket _callMessage = null;

        private SockClient _sockClient = null;

        /// <summary>
        /// 通讯信道中的客户端身份ID（外部业务标识）
        /// </summary>
        public string ClientId { get { return _sockClient == null ? string.Empty : _sockClient.ClientID; } }

        public CSockChannel(SockClient sockClient)
        {
            _sockClient = sockClient;
            if (!_sockClient.Connected) { throw new InvalidOperationException("调用失败：未连接到服务器！"); }

            _sockClient.OnServerReturned += _sockClient_OnMessageReturned;
        }

        private void _sockClient_OnMessageReturned(object sender, MessageReceivedEventArgs e)
        {
            if (e.ReceivedMessage.ReferId == _callMessage.Id)
            {
                //if ((DateTime.Now.Subtract(_callMessage.CreateTime).Milliseconds > _timeout)) { HasTimeout = true; return; }            //超时抛弃,已在SockClient中处理超时情况
                _remoteResponseData = e.ReceivedMessage.MessageBody;
                e.Processed = true;
                //Debug.WriteLine(string.Format("调用耗时：{0}ms", DateTime.Now.Subtract(_callMessage.CreateTime).TotalMilliseconds));
                _resetEvent.Set();
            }
            else
            {
                Console.WriteLine(string.Format("远程调用收到了不正确的回复，invoke id:{0}, refer id:{1}, received id:{2}", _callMessage.Id, e.ReceivedMessage.ReferId, e.ReceivedMessage.Id));
                //throw new InvalidOperationException(string.Format("远程调用收到了不正确的回复，invoke id:{0}, refer id:{1}, received id:{2}", _callMessage.Id, e.ReceivedMessage.ReferId, e.ReceivedMessage.Id));
            }
        }


        public string Invoke(int timeout, MessageDataPacket message)
        {
            if (message.MessageType != MessageType.Invoke) { throw new InvalidOperationException("该消息类型并不是属于远程调用"); }
            HasTimeout = false;
            _timeout = timeout;
            message.ClientId = _sockClient.ClientID;
            _callMessage = message;
            _resetEvent = new ManualResetEvent(false);
            _taskCancellation = new CancellationTokenSource();
            _commTask = new Task(new Action<object>(this.Send), message, _taskCancellation.Token);
            _commTask.Start();
            var flag = _resetEvent.WaitOne(_timeout);
            _sockClient.EndRemoteInvoking();
            HasTimeout = string.IsNullOrWhiteSpace(_remoteResponseData);
            if (HasTimeout)
            {
                if (OnTimeout != null) { OnTimeout(this, new ChannelTimeoutEventArgs(_callMessage) { ReturnMessage = message }); }
            }
            //this.Dispose();
            return _remoteResponseData;
        }


        private void Send(object obj)
        {
            var dataPacket = obj as MessageDataPacket;
            //保证远程调用的原子操作。客户端在多线程环境中，当前调用还未接收返回时又发起另一远程调用，会产生callMsg.Id和receiveMsg.ReferId不匹配的错误
            while (_sockClient.IsServerInvoking)
            {
                Thread.Sleep(100);
            }
            _sockClient.BeginRemoteInvoking(this);
            _sockClient.Send(dataPacket);
        }

        private bool _isDisposed = false;

        public void Dispose()
        {
            if (_isDisposed) { return; }
            HasTimeout = false;
            _callMessage = null;
            _remoteResponseData = null;
            _sockClient.OnServerReturned -= _sockClient_OnMessageReturned;
            if (_commTask != null && _taskCancellation != null)
            {
                _taskCancellation.Cancel();
                //while (_commTask.Status != TaskStatus.RanToCompletion)
                //{
                //    Thread.Sleep(100);
                //}
                _commTask.Dispose();
                _taskCancellation.Dispose();
            }
            _resetEvent.Dispose();
            _isDisposed = true;
        }
    }
}
