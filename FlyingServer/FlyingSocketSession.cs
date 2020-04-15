using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlyingSocket;
using FlyingSocket.Data;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace FlyingServer
{
    public class FlyingSocketSession : AppSession<FlyingSocketSession, BinaryRequestInfo>
    {
        /// <summary>
        /// 绑定到客户端上的Socket通讯信道
        /// </summary>
        public ISocketInvokingChannel CurrentChannel { get; private set; }
        /// <summary>
        /// 客户端标识ID
        /// </summary>
        public string ClientId { get; internal set; }
        /// <summary>
        /// 客户端主机名称
        /// </summary>
        public string HostName { get; internal set; }

        private long _remote_invoking_flag = 0;
        /// <summary>
        /// 标识此客户端会话当前是否正处于主动发起的远程同步调用的过程中
        /// </summary>
        public bool IsSyncInvoking { get { return this.CurrentChannel != null; /*Interlocked.Read(ref _remote_invoking_flag) > 0;*/ } }

        public void EndSyncInvoking()
        {
            this.CurrentChannel = null;
            Interlocked.Exchange(ref _remote_invoking_flag, 0L);        //设置开始远程调用 
        }


        /// <summary>
        /// 向客户端发送一个消息数据包
        /// </summary>
        /// <param name="dataPacket">数据包</param>
        /// <param name="channel">同步调用Socket通讯信道</param>
        public void Send(FlyingSocketPacket dataPacket, ISocketInvokingChannel channel = null)
        {
            if (dataPacket == null) { throw new ArgumentNullException("参数dataPacket不能为null。"); }
            if (string.IsNullOrEmpty(dataPacket.ClientId))
            {
                throw new ApplicationException("消息包没有指定客户端业务标识");
            }
            if (!string.Equals(dataPacket.ClientId, this.ClientId)) { throw new ArgumentException(string.Format("消息包发送对象错误。")); }
            var data = SerializationUtil.BinarySerialize(dataPacket);
            if (dataPacket.PacketType == SocketPacketType.Invoke)
            {
                //Interlocked.Exchange<ISockChannel>(ref _currentChannel, sockChannel);
                if (channel == null) { throw new ArgumentNullException("与客户端远程同步通讯时没有提供调用信道。"); }
                this.CurrentChannel = channel;
                Interlocked.Exchange(ref _remote_invoking_flag, 1L);        //设置开始远程调用
            }
            this.SendData(data);
        }

        private void SendData(byte[] dataBody)
        {
            if (!this.Connected) { return; }
            var commandData = Encoding.UTF8.GetBytes(FixedFlags.CMD_KEY);                           //协议命令只占4位（supersocket命令模式使用）
            var dataLen = BitConverter.GetBytes(dataBody.Length);                                   //int类型占4位，根据协议这里也只能4位，否则会出错
            var sendData = new byte[FixedFlags.HEAD_PACKET_SIZE + dataBody.Length];                 //命令加内容长度为8
            Array.ConstrainedCopy(commandData, 0, sendData, 0, 4);
            Array.ConstrainedCopy(dataLen, 0, sendData, 4, 4);
            Array.ConstrainedCopy(dataBody, 0, sendData, FixedFlags.HEAD_PACKET_SIZE, dataBody.Length);
            ArraySegment<byte> arraySegment = new ArraySegment<byte>(sendData);
            this.Send(arraySegment);

            //Array.Clear(commandData, 0, commandData.Length);            //liubq:发送后清除对内存及时回收有帮助（会减少内存开销），但极端高并发场景下，可能会造成客户端接收不到数据.服务端建议放开Array.Clear减轻Server内存压力
            //Array.Clear(dataBody, 0, dataBody.Length);
            //Array.Clear(dataLen, 0, dataLen.Length);
            //Array.Clear(sendData, 0, sendData.Length);
        }

        internal void StartAuth()
        {
            var start = DateTime.Now;
            Task.Factory.StartNew(() => {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                if (string.IsNullOrEmpty(ClientId))
                { 
                }
            });
        }
    }
}
