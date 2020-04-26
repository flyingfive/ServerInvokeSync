using System;

namespace SSock.Dependency
{
    /// <summary>
    /// 标识此类是一个socket服务的本地实现
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Obsolete("使用接口替代")]
    public class SockServiceAttribute : System.Attribute
    {

    }

    /// <summary>
    /// 标识此库是一个包括socket服务实现的程序集
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class SockServiceLibraryAttribute : System.Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SocketTimeoutAttribute : Attribute
    {
        /// <summary>
        /// Socket请求自定义超时时间（单位毫秒）
        /// </summary>
        public int Timeout { set; get; }
        public SocketTimeoutAttribute(int timeout)
        {
            Timeout = timeout;
        }
    }

}
