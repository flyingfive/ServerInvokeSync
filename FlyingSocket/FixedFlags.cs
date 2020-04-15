using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingSocket
{
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
}
