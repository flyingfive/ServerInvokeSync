using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;

namespace SSock.Server
{
    /// <summary>
    /// 客户端外部业务标识[内部sessionId和外部clientId关联]
    /// </summary>
    public class ExternalIdentification
    {
        /// <summary>
        /// socket会话ID
        /// </summary>
        public string SessionId { get; private set; }
        /// <summary>
        /// 客户端业务标识ID
        /// </summary>
        public string ClientId { get; private set; }
        /// <summary>
        /// 客户端主机名称
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 客户端远程地址
        /// </summary>
        public string ClientAddress { get; private set; }

        public ExternalIdentification(string sessionId, string clientId,string clientAddress)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) { throw new ArgumentException("参数：sessionId错误"); }
            if (string.IsNullOrWhiteSpace(clientId)) { throw new ArgumentException("参数：clientId错误"); }
            if (string.IsNullOrWhiteSpace(clientAddress)) { throw new ArgumentException("参数：clientAddress错误"); }
            this.SessionId = sessionId;
            this.ClientId = clientId;
            this.ClientAddress = clientAddress;
        }
    }

    public class ExternalIdentifiedEventArgs : EventArgs
    {
        /// <summary>
        /// 含外部标只的客户端信息
        /// </summary>
        public ExternalIdentification Client { get; private set; }

        public ExternalIdentifiedEventArgs(ExternalIdentification client)
        {
            if (client == null) { throw new ArgumentNullException("client不能为NULL"); }
            this.Client = client;
        }
    }

    public class ClientClosedEventArgs : EventArgs
    {
        /// <summary>
        /// 含外部标只的客户端信息
        /// </summary>
        public ExternalIdentification Client { get; private set; }
        public CloseReason CloseReason { get; set; }
        public ClientClosedEventArgs(ExternalIdentification client)
        {
            if (client == null) { throw new ArgumentNullException("client不能为NULL"); }
            this.Client = client;
        }
    }
}
