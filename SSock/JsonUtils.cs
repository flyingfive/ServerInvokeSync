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
        /// <summary>
        /// Json序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize(object obj);
        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <param name="json"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        object Deserialize(string json, Type dataType);
        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>

        T Deserialize<T>(string json);

    }
}
