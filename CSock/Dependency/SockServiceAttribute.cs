using System;

namespace CSock.Dependency
{

    /// <summary>
    /// 标识此类是一个socket服务的本地实现（IOC约定：必需要有一个接口，且接口名称格式为：I{className}
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SockServiceAttribute : System.Attribute
    {
        public SockServiceAttribute()
        {
        }
    }

    /// <summary>
    /// 标识此库是一个包括socket服务实现的程序集
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class SockServiceLibraryAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Socket远程调用信道中的超时时间
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SocketInvokeTimeoutAttribute : Attribute
    {
        /// <summary>
        /// Socket请求自定义超时时间（单位毫秒）
        /// </summary>
        public int Timeout { set; get; }
        public SocketInvokeTimeoutAttribute(int timeout)
        {
            Timeout = timeout;
        }
    }

}
