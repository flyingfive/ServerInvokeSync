using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock
{
    /// <summary>
    /// Json工具
    /// </summary>
    public class JsonUtils
    {
        private static Type _jsonSerializerType = null;
        private static object _syncObj = new object();
        /// <summary>
        /// 获取IJsonSerialization接口实例
        /// </summary>
        /// <returns></returns>
        public static IJsonSerialization GetSerializer()
        {
            if (_jsonSerializerType == null)
            {
                lock (_syncObj)
                {
                    if (_jsonSerializerType == null)
                    {
                        AppDomain.CurrentDomain.GetAssemblies()
                            .ToList().ForEach(assembly =>
                            {
                                if (_jsonSerializerType == null)
                                {
                                    _jsonSerializerType = assembly.GetTypes()
                                        .Where(t => t.IsClass && t.IsPublic && typeof(IJsonSerialization).IsAssignableFrom(t))
                                        .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                                        .Where(t => t.GetConstructor(Type.EmptyTypes).IsPublic).FirstOrDefault();
                                }
                            });
                    }
                }
            }
            if (_jsonSerializerType == null)
            {
                throw new InvalidOperationException("程序内没有指定IJsonSerialization实现");
            }
            return Activator.CreateInstance(_jsonSerializerType) as IJsonSerialization;
        }
    }

    /// <summary>
    /// 定义外部处理Json序列化的操作
    /// </summary>
    public interface IJsonSerialization
    {
        string Serialize(object obj);
        string Serialize(object[] array);

        object Deserialize(string json, Type dataType);

    }
}
