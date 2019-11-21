using CSock.Message;
using MsgPack.Serialization;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSock
{
    /// <summary>
    /// 固定头部数据接收过滤器，接收数据后粘包处理
    /// </summary>
    public class FixedHeadPacketReceiveFilter : FixedHeaderReceiveFilter<MessageDataPacket>
    {
        public FixedHeadPacketReceiveFilter() : base(FixedFlags.HEAD_PACKET_SIZE) { }

        public override MessageDataPacket ResolvePackage(IBufferStream bufferStream)
        {
            var bodyData = new byte[_bodyLength];
            var count = bufferStream.Skip(FixedFlags.HEAD_PACKET_SIZE).Read(bodyData, 0, _bodyLength);
            using (var stream = new MemoryStream(bodyData))
            {
                var serializer = SerializationContext.Default.GetSerializer<MessageDataPacket>();
                var message = serializer.Unpack(stream);
                bufferStream.Clear();
                //Array.Clear(bodyData, 0, bodyData.Length);
                return message;
            }
        }

        private int _bodyLength = 0;

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            var data = new byte[4];
            bufferStream.Read(data, 0, 4);          //前四个字节是command key,后四个字节是数据包长度
            var command = Encoding.UTF8.GetString(data);
            bufferStream.Read(data, 0, 4);
            var bodyLength = BitConverter.ToInt32(data, 0);
            _bodyLength = bodyLength;
            //Array.Clear(data, 0, 4);
            return bodyLength;
        }
    }
}
