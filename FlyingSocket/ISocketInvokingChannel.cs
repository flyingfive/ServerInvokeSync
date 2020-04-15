using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyingSocket.Data;

namespace FlyingSocket
{
    /// <summary>
    /// Socket调用通讯信道
    /// </summary>
    public interface ISocketInvokingChannel
    {
        /// <summary>
        /// 
        /// </summary>
        string ClientId { get; }        
        /// <summary>
        /// 信道通讯超时发生
        /// </summary>
        event EventHandler<EventArgs> Timeout;
        /// <summary>
        /// socket通信进行远程调用
        /// </summary>
        /// <param name="timeout">调用超时时间</param>
        /// <param name="message">调用数据</param>
        /// <returns></returns>
        byte[] Invoke(int timeout, FlyingSocketPacket invokeData);
        /// <summary>
        /// 信道通讯过程中是否发生超时
        /// </summary>
        bool HasTimeout { get; }

    }
}
