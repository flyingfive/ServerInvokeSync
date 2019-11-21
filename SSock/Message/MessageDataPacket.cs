using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock.Message
{
    /// <summary>
    /// socket消息数据包
    /// </summary>
    public class MessageDataPacket : IPackageInfo
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 关联消息ID，(消息Return时检查)
        /// </summary>
        public Guid ReferId { get; set; }
        /// <summary>
        /// 客户端业务身份标识
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 动作点(Invoke消息传递)
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 消息数据主体(JSON)
        /// </summary>
        public string MessageBody { get; set; }
        /// <summary>
        /// 消息生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MessageType { get; set; }

        public MessageDataPacket() { Id = Guid.NewGuid(); CreateTime = DateTime.Now; }

    }

    /// <summary>
    /// 固定的标识内容
    /// </summary>
    public class FixedFlags
    {
        /// <summary>
        /// 踢除客户端连接
        /// </summary>
        public const string ABANDON_CONN = "abandon_connection";
        /// <summary>
        /// 消息包头部固定8字节长度
        /// </summary>
        public const int HEAD_PACKET_SIZE = 8;
        /// <summary>
        /// socket命令键
        /// </summary>
        public const string CMD_KEY = "LIST";
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 心跳数据
        /// </summary>
        HeartBeat,
        /// <summary>
        /// (方法)调用消息
        /// </summary>
        Invoke,
        /// <summary>
        /// (方法)返回消息
        /// </summary>
        Return,
        /// <summary>
        /// 通知消息(无需返回)
        /// </summary>
        Notice,
        /// <summary>
        /// 标识客户端业务身份
        /// </summary>
        Identity,
    }
}
