using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSock.Dependency;
using SSock.Server;

namespace WebUI.App_Start
{
    public class SockServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var contracts = new List<Type>();
            //AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.IsDefined(typeof(SockServiceLibraryAttribute), false))
            Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.IsDefined(typeof(SockServiceAttribute), false) && t.GetInterfaces().Any(i => string.Equals(i.Name, string.Format("I{0}", t.Name))))
                        .ToList()
                        .ForEach(implementType =>
                        {
                            var serviceType = implementType.GetInterfaces().Where(i => i.Name.Equals(string.Format("I{0}", implementType.Name))).FirstOrDefault();
                            container.Register(Component.For(serviceType).ImplementedBy(implementType).LifestyleTransient().Named(serviceType.FullName));
                        });
        }
    }

    public class SocketHost
    {
        private SockServer _server = null;

        public SockServer Server { get { return _server; } }

        private static SocketHost _host = null;

        public static Castle.Windsor.WindsorContainer _container = null;
        private static object _syncObj = new object();

        public static SocketHost Instance
        {
            get
            {
                if (_host == null)
                {
                    lock (_syncObj)
                    {
                        if (_host == null) { _host = new SocketHost(); }
                    }
                }
                return _host;
            }
        }


        static SocketHost()
        {
            _container = new Castle.Windsor.WindsorContainer();
        }

        public void InstallService()
        {
            _container.Install(new SockServiceInstaller());
        }

        private SocketHost()
        {
            _server = SockServerFactory.Build();
            _server.Started += _server_Started;
            _server.OnClientInvoking += _server_OnRemoteInvoking;
            _server.NewSessionConnected += _server_NewSessionConnected;
            _server.OnMessageReceived += _server_OnMessageReceived;
            //_server.OnClientClosed += _server_OnClientClosed;
            _server.OnClientClosed += _server_OnClientClosed;
            _server.OnClientIdentified += _server_OnClientIdentified;
            //_server.OnClientIdentified += _server_OnClientIdentified;
        }

        private void _server_OnClientClosed(object sender, ClientClosedEventArgs e)
        {
            //e.Client.StartTime
        }

        private void _server_OnClientIdentified(object sender, ClientSocketEventArgs e)
        {
            throw new NotImplementedException();
        }

        //private void _server_OnClientIdentified(object sender, ExternalIdentifiedEventArgs e)
        //{
        //    var client = e.Client;
        //    System.Diagnostics.Debug.WriteLine(string.Format("{0}|{1}|{2}", client.ClientId, client.SessionId, client.ClientAddress));
        //}


        private void _server_OnRemoteInvoking(object sender, InvokeMessageEventArgs e)
        {
            var routeData = e.InvokeMessage.Action.Split(new char[] { ':' });
            var serviceName = routeData.First();
            var actionName = routeData.Last();
            var service = _container.Resolve(serviceName, typeof(Object));
            var method = service.GetType().GetMethod(actionName);
            var args = new List<Object>();
            var parameters = JArray.Parse(e.InvokeMessage.MessageBody);
            var i = 0;
            foreach (var p in method.GetParameters())
            {
                if (p.ParameterType == typeof(string))
                {
                    var val = parameters[i].Type == JTokenType.Null ? null : parameters[i].ToString();
                    args.Add(val);
                }
                else
                {
                    args.Add(JsonConvert.DeserializeObject(parameters[i].ToString(), p.ParameterType));
                }
                i++;
            }
            var retValue = method.Invoke(service, args.ToArray());
            e.ReturnData = retValue;
        }


        private void _server_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {

        }

        //private void _server_SessionClosed(SockSession session, SuperSocket.SocketBase.CloseReason value)
        //{
        //    //var item = lbClients.Items.OfType<string>().FirstOrDefault(x => x.StartsWith(session.SessionID));
        //    //if (item != null)
        //    //{
        //    //    DisplayClients(item, false);
        //    //}
        //    var client = _server.OnlineClients.Values.Where(s => string.Equals(s.SessionId, session.SessionID)).FirstOrDefault();
        //    if (client != null)
        //    {
        //        System.Diagnostics.Debug.WriteLine(string.Format("客户端：{0}断开连接，原因：{1}", client.ClientId, value.ToString()));
        //    }
        //}

        //private void _server_OnClientClosed(object sender, ClientClosedEventArgs e)
        //{
        //    //RemoveClient(e.Client, e.CloseReason);
        //}

        private void _server_NewSessionConnected(SockSession session)
        {
            if (session.Connected)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}|{1}", session.SessionID, session.RemoteEndPoint.ToString()), true);
            }
        }

        private void _server_Started(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("服务启动成功！");
        }
    }
}