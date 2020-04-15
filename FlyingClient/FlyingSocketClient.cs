using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlyingSocket;
using FlyingSocket.Data;
using SuperSocket.ClientEngine;

namespace FlyingClient
{
    public class FlyingSocketClient : AsyncTcpSession
    {
        public string Id { get; private set; }
        /// <summary>
        /// 一个完整的通知类消息收到后触发
        /// </summary>
        public event EventHandler<FlyingPacketReceivedEventArgs> OnMessageReceived;
        /// <summary>
        /// 结束向服务端发起的远程同步调用。（本地客户端主动请求）
        /// </summary>
        public event EventHandler<FlyingPacketReceivedEventArgs> EndRemoteInvoking;

        /// <summary>
        /// 开始处理服务端的远程同步调用，需要即时处理并返回数据。（服务端主动请求）
        /// </summary>
        public event EventHandler<FlyingPacketReceivedEventArgs> BeginRemoteInvoking;
        /// <summary>
        /// 标识客户端当前是否正处于主动发起的远程同步调用的过程中
        /// </summary>
        public bool IsSyncInvoking { get { return this.CurrentChannel != null;/*Interlocked.Read(ref _remote_invoking_flag) > 0;*/ } }

        //远程调用标识，设计为静态表示全局并发控制。这里为实例字段，控制单个SockClient实例对象不能同时并发处理远程的Server调用。必需在一个调用收到回复或超时后再发起第二次远程调用信道
        private long _remote_invoking_flag = 0;

        public FlyingSocketClient(string clientId):base()
        {
            if (string.IsNullOrWhiteSpace(clientId)) { throw new ArgumentException(); }
            this.Id = clientId;

        }

        /// <summary>
        /// 绑定到客户端上的Socket通讯信道
        /// </summary>
        public ISocketInvokingChannel CurrentChannel { get; private set; }

        /// <summary>
        /// 发送一个标准的通讯消息数据包
        /// </summary>
        /// <param name="dataPacket"></param>
        /// <param name="channel">同步调用Socket通讯信道</param>
        public void Send(FlyingSocketPacket dataPacket, ISocketInvokingChannel channel = null)
        {
            if (dataPacket.PacketType == SocketPacketType.Invoke)
            {
                if (channel == null) { throw new ArgumentNullException("与服务端远程同步通讯时没有提供调用信道。"); }
                //Interlocked.Exchange<ISocketInvokingChannel>(ref _currentChannel, sockChannel);
                this.CurrentChannel = channel;
                Interlocked.Exchange(ref _remote_invoking_flag, 1L);
            }
            var data = SerializationUtil.BinarySerialize(dataPacket);
            base.Send(data, 0, data.Length);
            //using (var stream = new MemoryStream())
            //{
            //    var serializer = SerializationContext.Default.GetSerializer<MessageDataPacket>();
            //    serializer.Pack(stream, dataPacket);
            //    var bodyData = stream.ToArray();
            //    this.Send(bodyData);
            //}
        }

        /// <summary>
        /// 标识服务端的远程调用结束
        /// </summary>
        public void EndSyncInvoking()
        {
            if (!IsSyncInvoking) { return; }
            //Interlocked.Exchange<ISocketInvokingChannel>(ref _currentChannel, null);
            this.CurrentChannel = null;
            Interlocked.Exchange(ref _remote_invoking_flag, 0L);
        }

        protected override void OnDataReceived(byte[] data, int offset, int length)
        {
            base.OnDataReceived(data, offset, length);

        }

        public override void Connect(EndPoint remoteEndPoint)
        {
            base.Connect(remoteEndPoint);
        }

        //override 
    }
}
