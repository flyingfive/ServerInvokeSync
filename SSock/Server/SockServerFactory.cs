using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock.Server
{
    /// <summary>
    /// socket服务工厂
    /// </summary>
    public static class SockServerFactory
    {
        private static Dictionary<string, SockServer> _servers = null;
        private static object _locker = new object();

        static SockServerFactory()
        {
            _servers = new Dictionary<string, SockServer>();
        }

        public static SockServer GetServerByName(string serverName)
        {
            if (_servers.ContainsKey(serverName))
            {
                lock (_locker)
                {
                    if (_servers.ContainsKey(serverName))
                    {
                        return _servers[serverName];
                    }
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 使用程序配置文件构建socket服务
        /// </summary>
        /// <returns></returns>
        public static SockServer Build()
        {
            var param = SockConfig.GetConfig();
            if (!_servers.ContainsKey(param.ServerName))
            {
                lock (_locker)
                {
                    if (!_servers.ContainsKey(param.ServerName))
                    {
                        var server = new SockServer();
                        server.Setup(param);
                        _servers.Add(param.ServerName, server);
                        return server;
                    }
                    throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));
                }
            }
            throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));
        }

        /// <summary>
        /// 使用指定参数配置构建socket服务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static SockServer Build(SockConfig param)
        {
            if (!_servers.ContainsKey(param.ServerName))
            {
                lock (_locker)
                {
                    if (!_servers.ContainsKey(param.ServerName))
                    {
                        var server = new SockServer();
                        server.Setup(param);
                        _servers.Add(param.ServerName, server);
                        return server;
                    }
                    throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));
                }
            }
            throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", param.ServerName));

        }

        /// <summary>
        /// 使用具体的端口、名称快速构建socket服务，其它配置使用默认值
        /// </summary>
        /// <param name="port"></param>
        /// <param name="serverName"></param>
        /// <param name="maxRequestLength"></param>
        /// <returns></returns>
        public static SockServer Build(int port, string serverName, int maxRequestLength = int.MaxValue)
        {
            if (port < 1000 || port > 60000)
            {
                throw new ArgumentException("有效端口范围在1000~60000之间");
            }
            var sissServer = new SockServer();
            if (sissServer.State != SuperSocket.SocketBase.ServerState.NotInitialized)
            {
                throw new InvalidOperationException("启动失败：socket服务不在未初始化状态下。");
            }

            if (!_servers.ContainsKey(serverName))
            {
                lock (_locker)
                {
                    if (!_servers.ContainsKey(serverName))
                    {
                        if (maxRequestLength < 1) { maxRequestLength = int.MaxValue; }
                        string maxStr = System.Configuration.ConfigurationManager.AppSettings["MaxClientCount"];
                        int maxCount = 1000;
                        int.TryParse(maxStr, out maxCount);

                        string maxBufferStr = System.Configuration.ConfigurationManager.AppSettings["MaxBufferSize"];
                        int maxBuffer = 100;
                        int.TryParse(maxBufferStr, out maxBuffer);
                        maxBuffer = maxBuffer * 1024 * 1;
                        var cfg = new ServerConfig()
                        {
                            Name = serverName,
                            Port = Convert.ToInt32(port),
                            LogAllSocketException = true,
                            MaxRequestLength = maxRequestLength,
                            MaxConnectionNumber = maxCount,
                            SendBufferSize = maxBuffer,
                            ReceiveBufferSize = maxBuffer
                        };
                        var flag = sissServer.Setup(cfg);
                        if (!flag)
                        {
                            throw new InvalidOperationException("启动失败：端口设置失败！");
                        }
                        return sissServer;
                    }
                    throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", serverName));
                }
            }
            throw new InvalidOperationException(string.Format("已存在名称为：{0}的SockServer!", serverName));
        }
    }
}
