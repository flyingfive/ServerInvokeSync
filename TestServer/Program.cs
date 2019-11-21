using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SSock.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

[assembly: SockServiceLibrary]

namespace TestServer
{
    static class Program
    {
        public static Castle.Windsor.WindsorContainer _container = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _container = new Castle.Windsor.WindsorContainer();
            _container.Install(new SockServiceInstaller());
            Application.Run(new FrmServer());
        }
    }

    public class SockServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var contracts = new List<Type>();
            AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.IsDefined(typeof(SockServiceLibraryAttribute), false))
                .ToList()
                .ForEach(assem =>
                {
                    assem.GetTypes().Where(t => t.IsClass && t.IsDefined(typeof(SockServiceAttribute), false) && t.GetInterfaces().Any(i => string.Equals(i.Name, string.Format("I{0}", t.Name))))
                        .ToList()
                        .ForEach(implementType => {
                            var serviceType = implementType.GetInterfaces().Where(i => i.Name.Equals(string.Format("I{0}", implementType.Name))).FirstOrDefault();
                            container.Register(Component.For(serviceType).ImplementedBy(implementType).LifestyleTransient().Named(serviceType.FullName));
                        });
                });
        }
    }

}
