using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SSock.Server
{
    public sealed class FlyingServerConfig : ConfigurationSection
    {
        public static FlyingServerConfig GetConfig()
        {
            return GetConfig("SockConfig");
        }

        public static FlyingServerConfig GetConfig(string sectionName)
        {
            var section = (FlyingServerConfig)ConfigurationManager.GetSection(sectionName);
            if (section == null)
                return new FlyingServerConfig();
            return section;
        }

        /// <summary>
        /// 最大连接数限制，默认1000
        /// </summary>
        [ConfigurationProperty("MaxConnectionNumber", IsRequired = false, DefaultValue = 1000)]
        public int MaxConnectionNumber { get { return (int)base["MaxConnectionNumber"]; } set { base["MaxConnectionNumber"] = value; } }

        /// <summary>
        /// 接收和发送缓冲区大小,单位KB，默认100
        /// </summary>
        [ConfigurationProperty("BufferSize", IsRequired = false, DefaultValue = 1024 * 100)]
        public int BufferSize { get { return (int)base["BufferSize"]; } set { base["BufferSize"] = value; } }

        /// <summary>
        /// 最大请求长度
        /// </summary>
        [ConfigurationProperty("MaxRequestLength", IsRequired = false, DefaultValue = int.MaxValue)]
        public int MaxRequestLength { get { return (int)base["MaxRequestLength"]; } set { base["MaxRequestLength"] = value; } }

        /// <summary>
        /// 服务名称
        /// </summary>
        [ConfigurationProperty("ServerName", IsRequired = false, DefaultValue = "DefaultSockServer")]
        public string ServerName { get { return base["ServerName"].ToString(); } set { base["ServerName"] = value; } }

        /// <summary>
        /// 服务端口
        /// </summary>
        [ConfigurationProperty("Port", IsRequired = true, DefaultValue = 52520)]
        public int Port { get { return (int)base["Port"]; } set { base["Port"] = value; } }
        /// <summary>
        /// 踢除哪个重复的客户端连接[Previous:前一个,Current:当前的]
        /// </summary>
        [ConfigurationProperty("AbandonDuplicateClient", IsRequired = false, DefaultValue = "Previous")]
        public string AbandonDuplicateClient { get { return base["AbandonDuplicateClient"].ToString(); } set { base["AbandonDuplicateClient"] = value; } }

        /// <summary>
        /// 远程调用超时时间（单位毫秒，默认15）
        /// </summary>
        [ConfigurationProperty("RemoteInvokeTimeout", IsRequired = false, DefaultValue = 15000)]
        public int RemoteInvokeTimeout { get { return (int)base["RemoteInvokeTimeout"]; } set { base["RemoteInvokeTimeout"] = value; } }
        /// <summary>
        /// 网络连接正常情况下的keep alive数据的发送间隔（单位秒，默认60）
        /// </summary>
        [ConfigurationProperty("KeepAliveTime", IsRequired = false, DefaultValue = 60)]
        public int KeepAliveTime { get { return (int)base["KeepAliveTime"]; } set { base["KeepAliveTime"] = value; } }
    }
}
