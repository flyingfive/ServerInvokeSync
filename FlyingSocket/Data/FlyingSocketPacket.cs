using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingSocket.Data
{
    /// <summary>
    /// 一次Socket通信数据包
    /// </summary>
    public class FlyingSocketPacket
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
        /// 消息数据主体
        /// </summary>
        public byte[] Body { get; set; }
        /// <summary>
        /// 消息生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 包类型
        /// </summary>
        public SocketPacketType PacketType { get; set; }
    }


    /// <summary>
    /// 消息类型
    /// </summary>
    public enum SocketPacketType
    {
        /// <summary>
        /// 心跳数据
        /// </summary>
        HeartBeat,
        /// <summary>
        /// 即时(方法)调用/请求消息
        /// </summary>
        Invoke,
        /// <summary>
        /// 即时(方法)返回/响应消息
        /// </summary>
        Return,
        /// <summary>
        /// 通知消息(无需返回)
        /// </summary>
        Notice,
        /// <summary>
        /// 客户端身份
        /// </summary>
        [Obsolete("换方式实现身份认证。")]
        Authentication,
    }
}
