using SSock.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock
{
    /// <summary>
    /// socket通讯信道
    /// </summary>
    public interface ISockChannel : IDisposable
    {
        /// <summary>
        /// 信道通讯超时发生
        /// </summary>
        event EventHandler<ChannelTimeoutEventArgs> OnTimeout;
        /// <summary>
        /// socket通信进行远程调用
        /// </summary>
        /// <param name="timeout">调用超时时间</param>
        /// <param name="message">一个标准的远程调用消息数据包</param>
        /// <returns></returns>
        string Invoke(int timeout, MessageDataPacket message);
        /// <summary>
        /// 信道通讯过程中是否发生超时
        /// </summary>
        bool HasTimeout { get; }
    }

    public class ChannelTimeoutEventArgs : EventArgs
    {
        public MessageDataPacket CallMessage { get; private set; }

        public MessageDataPacket ReturnMessage { get; set; }

        public ChannelTimeoutEventArgs(MessageDataPacket dataPacket) { CallMessage = dataPacket; }
    }
}
