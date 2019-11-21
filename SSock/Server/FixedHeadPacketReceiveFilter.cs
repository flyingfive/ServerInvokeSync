using SSock.Message;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock.Server
{
    /// <summary>
    /// 头部定长数据包接收过滤器
    /// </summary>
    public class FixedHeadPacketReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        public FixedHeadPacketReceiveFilter() : base(FixedFlags.HEAD_PACKET_SIZE) { }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var headerData = new byte[4];
            Array.Copy(header, offset + 4, headerData, 0, 4);
            var bodyLength = BitConverter.ToInt32(headerData, 0);
            //Array.Clear(header, 0, 4);
            return bodyLength;
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            var key = Encoding.UTF8.GetString(header.Array, header.Offset, 4);
            var info = new BinaryRequestInfo(key, bodyBuffer.CloneRange(offset, length));
            //Array.Clear(bodyBuffer, 0, bodyBuffer.Length);
            return info;
        }
    }
}
