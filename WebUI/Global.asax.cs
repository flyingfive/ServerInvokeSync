using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SSock.Server;
using WebUI.App_Start;

namespace WebUI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SocketHost.Instance.InstallService();
            var config = SockConfig.GetConfig();
            while (CheckNetPortUsed(config.Port))
            {
                Thread.Sleep(2000);
            }
            SocketHost.Instance.Server.Start();
        }


        /// <summary>
        /// 检查端口是否被使用
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        private static bool CheckNetPortUsed(int port)
        {
            bool flag = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();//获取所有的监听连接
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
    }
}
