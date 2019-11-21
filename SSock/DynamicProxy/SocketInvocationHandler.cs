//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using SSock.Dependency;
using SSock.Message;
using SSock.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SSock.DynamicProxy
{
    /// <summary>
    /// 通过Socket通讯进行代理调用的接口实现处理点
    /// </summary>
    public class SocketInvocationHandler : IProxyInvocationHandler
    {
        //private ISockChannel _sockChannel = null;
        private IJsonSerialization _jsonHelper = null;
        private string _clientId = null;

        public SocketInvocationHandler(string clientId)
        {
            _clientId = clientId;
            _jsonHelper = JsonUtils.GetSerializer();
        }

        /// <summary>
        /// 代理远程调用
        /// </summary>
        /// <param name="proxy">代理对象</param>
        /// <param name="method">代理的方法</param>
        /// <param name="parameters">调用方法的参数信息</param>
        /// <returns></returns>
        public Object Invoke(Object proxy, MethodInfo method, Object[] parameters)
        {
            var json = _jsonHelper.Serialize(parameters);// JArray.FromObject(parameters).ToString();
            var packet = new MessageDataPacket()
            {
                MessageType = MessageType.Invoke,
                MessageBody = json,
                Action = string.Format("{0}:{1}", method.DeclaringType.FullName, method.Name),
            };
            //超时时间：默认值->全局值(配置)->个例值(SocketInvokeTimeoutAttribute)
            var timeout = SockConfig.GetConfig().RemoteInvokeTimeout;
            var attribute = method.GetCustomAttributes(typeof(SocketTimeoutAttribute), false).FirstOrDefault() as SocketTimeoutAttribute;
            if (attribute != null)
            {
                timeout = attribute.Timeout;
            }
            using (var sockChannel = new SSockChannel(_clientId))
            {
                var responseJson = sockChannel.Invoke(timeout, packet);
                if (string.IsNullOrEmpty(responseJson))
                {
                    if (method.ReturnType.IsValueType)
                    {
                        var name = packet.Action.IndexOf(":") > 0 ? packet.Action.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Last() : packet.Action;
                        throw new ApplicationException(string.Format("对于值类型的远程调用:{0}返回了空值!", name));
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var retValue = _jsonHelper.Deserialize(responseJson, method.ReturnType);//JsonConvert.DeserializeObject(responseJson, method.ReturnType);
                    return retValue;
                }
            }
        }
    }
}
