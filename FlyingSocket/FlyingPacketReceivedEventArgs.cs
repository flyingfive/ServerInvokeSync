using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyingSocket.Data;

namespace FlyingSocket
{
    public class FlyingPacketReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 消息是否已被处理
        /// </summary>
        public bool Processed { get; set; }
        /// <summary>
        /// 收到的消息
        /// </summary>
        public FlyingSocketPacket ReceivedMessage { get; private set; }

        public FlyingPacketReceivedEventArgs(FlyingSocketPacket messageDataPacket)
        {
            if (messageDataPacket == null) { throw new ArgumentNullException(); }
            ReceivedMessage = messageDataPacket;
        }
    }


    /// <summary>
    /// 远程同步调用事件参数
    /// </summary>
    public class RemoteInvokingEventArgs : EventArgs
    {
        /// <summary>
        /// 接收到的远程调用方法
        /// </summary>
        public FlyingSocketPacket DataPacket { get; private set; }
        /// <summary>
        /// 需要返回（回复）的数据
        /// </summary>
        public object ReturnData { get; set; }

        public RemoteInvokingEventArgs(FlyingSocketPacket dataPacket)
        {
            if (dataPacket == null) { throw new ArgumentNullException(); }
            DataPacket = dataPacket;
        }
    }

    /// <summary>
    /// Socket同步通讯信道中的超时事件参数
    /// </summary>
    public class SyncSocketChannelTimeoutEventArgs : EventArgs
    {
        /// <summary>
        /// 发送出去的消息包
        /// </summary>
        public FlyingSocketPacket SendedPacket { get; private set; }
        /// <summary>
        /// 返回来时已被超时遗弃的消息包(如果存在)
        /// </summary>
        public FlyingSocketPacket AbandonedPacket { get; set; }

        public SyncSocketChannelTimeoutEventArgs(FlyingSocketPacket dataPacket) { SendedPacket = dataPacket; }
    }


    /// <summary>
    /// Socket客户端信息
    /// </summary>
    public class SocketClientInfo
    {
        /// <summary>
        /// 客户端身份ID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// SuperSocket会话ID
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// 远程地址
        /// </summary>
        public string RemoteAddress { get; set; }
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }
        /// <summary>
        /// 连接创建时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }
}
