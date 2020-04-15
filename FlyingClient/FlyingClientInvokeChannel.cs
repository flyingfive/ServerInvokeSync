using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlyingSocket;
using FlyingSocket.Data;

namespace FlyingClient
{
    public class FlyingClientInvokeChannel : ISocketInvokingChannel, IDisposable
    {
        private FlyingSocketClient _socketClient = null;
        public string ClientId { get { return _socketClient == null ? string.Empty : _socketClient.Id; } }

        public bool HasTimeout { get; private set; }

        private int _timeout = 0;   //超时值，单位毫秒
        public event EventHandler<EventArgs> Timeout;
        private Task _commTask = null;                              //异步通讯任务
        private CancellationTokenSource _taskCancellation = null;   //通讯任务线程的取消标记
        //private string _remoteResponseData = string.Empty;          //远程服务端响应数据
        private ManualResetEvent _resetEvent = null;
        private FlyingSocketPacket _invokeSendPacket = null;
        private FlyingSocketPacket _invokeReceivePacket = null;

        public FlyingClientInvokeChannel(FlyingSocketClient socketClient)
        {
            if (socketClient == null) { throw new ArgumentNullException(); }
            if (socketClient.IsConnected) { throw new InvalidOperationException("调用失败：未连接到服务器！"); }
            _socketClient = socketClient;
            _socketClient.EndRemoteInvoking += _socketClient_EndRemoteInvoking;
        }

        private void _socketClient_EndRemoteInvoking(object sender, FlyingPacketReceivedEventArgs e)
        {
            if (e.ReceivedMessage.ReferId == _invokeSendPacket.Id)
            {
                //if ((DateTime.Now.Subtract(_callMessage.CreateTime).Milliseconds > _timeout)) { HasTimeout = true; return; }            //超时抛弃,已在SockClient中处理超时情况
                //_remoteResponseData = e.ReceivedMessage.MessageBody;
                _invokeReceivePacket = e.ReceivedMessage;
                e.Processed = true;
                //Debug.WriteLine(string.Format("调用耗时：{0}ms", DateTime.Now.Subtract(_callMessage.CreateTime).TotalMilliseconds));
                _resetEvent.Set();
            }
            else
            {
                Console.WriteLine(string.Format("远程调用收到了不正确的回复，invoke id:{0}, refer id:{1}, received id:{2}", _invokeSendPacket.Id, e.ReceivedMessage.ReferId, e.ReceivedMessage.Id));
                //throw new InvalidOperationException(string.Format("远程调用收到了不正确的回复，invoke id:{0}, refer id:{1}, received id:{2}", _callMessage.Id, e.ReceivedMessage.ReferId, e.ReceivedMessage.Id));
            }
        }

        public byte[] Invoke(int timeout, FlyingSocketPacket invokeData)
        {
            if (invokeData.PacketType != SocketPacketType.Invoke) { throw new InvalidOperationException("该消息类型并不是属于远程同步调用"); }
            HasTimeout = false;
            _timeout = timeout;
            invokeData.ClientId = _socketClient.Id;
            _invokeSendPacket = invokeData;
            _resetEvent = new ManualResetEvent(false);
            _taskCancellation = new CancellationTokenSource();
            _commTask = new Task(new Action(this.Send), _taskCancellation.Token);
            _commTask.Start();
            var flag = _resetEvent.WaitOne(_timeout);
            _socketClient.EndSyncInvoking();
            HasTimeout = _invokeReceivePacket == null;
            if (HasTimeout)
            {
                if (Timeout != null) { Timeout(this, new SyncSocketChannelTimeoutEventArgs(_invokeSendPacket) { AbandonedPacket = _invokeReceivePacket }); }
            }
            //this.Dispose();
            return HasTimeout ? new byte[0] : _invokeReceivePacket.Body;
        }
        private void Send()
        {
            //保证远程调用的原子操作。客户端在多线程环境中，当前调用还未接收返回时又发起另一远程调用，会产生callMsg.Id和receiveMsg.ReferId不匹配的错误
            while (_socketClient.IsSyncInvoking)
            {
                Thread.Sleep(100);
            }
            _socketClient.Send(_invokeSendPacket);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
