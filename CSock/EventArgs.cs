using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSock.Message;

namespace CSock
{
    /// <summary>
    /// 本地到服务端远程调用后接收到返回消息包时的事件参数
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 消息是否已被处理
        /// </summary>
        public bool Processed { get; set; }
        /// <summary>
        /// 收到的消息
        /// </summary>
        public MessageDataPacket ReceivedMessage { get; private set; }

        public MessageReceivedEventArgs(MessageDataPacket messageDataPacket)
        {
            ReceivedMessage = messageDataPacket;
        }
    }
    /// <summary>
    /// 接收到远程调用消息时的事件参数
    /// </summary>
    public class InvokeMessageEventArgs : EventArgs
    {
        /// <summary>
        /// 远程调用消息包
        /// </summary>
        public MessageDataPacket InvokeMessage { get; private set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object ReturnData { get; set; }

        public InvokeMessageEventArgs(MessageDataPacket dataPacket) { InvokeMessage = dataPacket; }
    }
}
