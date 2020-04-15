using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingSocket.Data
{
    public class SerializationUtil
    {
        public static byte[] BinarySerialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                var b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                b.Serialize(ms, obj);
                return ms.ToArray();
            }
        }


        /// <summary>
        /// 从二进制数组还原对象
        /// </summary>
        /// <param name="data">对象的二进制格式数据</param>
        /// <returns></returns>
        public static T BinaryDeserialize<T>(byte[] data) //where T : class
        {
            T obj = default(T);
            using (var ms = new MemoryStream(data))
            {
                var b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                obj = (T)b.Deserialize(ms); //as T;
            }
            return obj;
        }

    }
}
