using MsgPack.Serialization;
using SSock.Message;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SSock.Server
{
    /// <summary>
    /// 客户端连接会话
    /// </summary>
    public class SockSession : AppSession<SockSession, BinaryRequestInfo>
    {
        private long _remote_invoking_flag = 0;
        /// <summary>
        /// 标识此客户端会话当前是否正处于远程调用的过程中
        /// </summary>
        public bool IsClientInvoking { get { return Interlocked.Read(ref _remote_invoking_flag) > 0; } }

        private ISockChannel _currentChannel = null;
        /// <summary>
        /// 绑定到客户端上的Socket通讯信道
        /// </summary>
        public ISockChannel CurrentChannel { get { return _currentChannel; } }
        /// <summary>
        /// 外部业务客户端ID
        /// </summary>
        public string ClientID { get; internal set; }
        /// <summary>
        /// 客户端主机名称
        /// </summary>
        public string HostName { get; internal set; }
        protected override void HandleUnknownRequest(BinaryRequestInfo requestInfo)
        {
            this.Send("unknown message.");
            Logger.Error(string.Format("sessionID:{0}接收到未知消息。KEY:{1}，ClientID:{2}, LastActiveTime:{3}, StartTime:{4}。"
                , this.SessionID, requestInfo.Key, this.ClientID ?? ""
                , this.LastActiveTime.ToString("yyyy-MM-dd HH:mm:ss")
                , this.StartTime.ToString("yyyy-MM-dd HH:mm:ss")));
        }
        protected override void HandleException(Exception e)
        {
            Logger.Error("将关闭Socket连接，发生错误：", e);
            this.Close(CloseReason.ApplicationError);
        }

        /// <summary>
        /// 标识结束客户端远程调用
        /// </summary>
        public void EndRemoteInvoking()
        {
            Interlocked.Exchange<ISockChannel>(ref _currentChannel, null);
            Interlocked.Exchange(ref _remote_invoking_flag, 0L);        //设置开始远程调用
        }

        /// <summary>
        /// 标识开始客户端的远程调用
        /// </summary>
        public void BeginRemoteInvoking(ISockChannel sockChannel)
        {
            if (sockChannel == null) { throw new ArgumentNullException("sockChannel不能为NULL!"); }
            Interlocked.Exchange<ISockChannel>(ref _currentChannel, sockChannel);
            Interlocked.Exchange(ref _remote_invoking_flag, 1L);        //设置开始远程调用
        }

        /// <summary>
        /// 向客户端发送一个消息数据包
        /// </summary>
        /// <param name="dataPacket"></param>
        public void Send(MessageDataPacket dataPacket)
        {
            if (string.IsNullOrEmpty(dataPacket.ClientId))
            {
                throw new ApplicationException("消息包没有指定客户端业务标识");
            }
            using (var stream = new MemoryStream())
            {
                var serializer = SerializationContext.Default.GetSerializer<MessageDataPacket>();
                serializer.Pack(stream, dataPacket);
                var bodyData = stream.ToArray();
                this.SendData(bodyData);
            }
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
    }
}
