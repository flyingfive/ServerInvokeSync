using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;

namespace SSock.Server
{
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

    public class ClientSocketEventArgs : EventArgs
    {
        public SocketClientInfo Client { get; protected set; }

        public ClientSocketEventArgs(SocketClientInfo clientInfo)
        {
            if (clientInfo == null) { throw new ArgumentNullException("参数clientInfo不能为null."); }
            this.Client = clientInfo;
        }
    }

    public class ClientClosedEventArgs : ClientSocketEventArgs
    {
        public CloseReason CloseReason { get; private set; }
        public ClientClosedEventArgs(CloseReason reason, SocketClientInfo clientInfo):base(clientInfo)
        {
            this.CloseReason = reason;
        }
    }
}
