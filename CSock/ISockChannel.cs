using CSock.Message;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSock
{
    /// <summary>
    /// socket信道
    /// </summary>
    public interface ISockChannel : IDisposable
    {
        string ClientId { get; }
        /// <summary>
        /// 信道通讯超时
        /// </summary>
        event EventHandler<ChannelTimeoutEventArgs> OnTimeout;
        /// <summary>
        /// 通过socket远程调用
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="message"></param>
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

    public class SockChannelFactory
    {
        public static ConcurrentDictionary<string, ISockChannel> _channels = new ConcurrentDictionary<string, ISockChannel>();
        //public static ISockChannel CreateChannel(SockClient client)
        //{
        //    ISockChannel channel = null;
        //    if (_channels.TryGetValue(client.ClientID, out channel))
        //    {
        //        channel.
        //    }
        //}
    }
}
