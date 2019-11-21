using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock.Server
{
    public class DefaultSockServer
    {
        private static object _lockObj = new object();

        private static SockServer _instance = null;

        /// <summary>
        /// 获取服务端默认实例
        /// </summary>
        public static SockServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = SockServerFactory.Build();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
