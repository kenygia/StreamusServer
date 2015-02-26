﻿using Autofac;
using Autofac.Integration.WebApi;
using log4net;
using NHibernate;
using Streamus_Web_API.Domain.Interfaces;
using Streamus_Web_API.Domain.Managers;
using System.Reflection;
using System.Web.Http;

namespace Streamus_Web_API.Dao
{
    public class AutofacRegistrations
    {
        public static void RegisterAndSetResolver(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            containerBuilder.Register(x => new NHibernateConfiguration().Configure().BuildSessionFactory()).SingleInstance();

            containerBuilder.RegisterType<NHibernateDaoFactory>().As<IDaoFactory>().InstancePerApiRequest();
            containerBuilder.RegisterType<StreamusManagerFactory>().As<IManagerFactory>().InstancePerApiRequest();
            containerBuilder.Register(x => x.Resolve<ISessionFactory>().OpenSession()).InstancePerApiRequest();
            containerBuilder.Register(x => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)).InstancePerApiRequest();
            
            ILifetimeScope container = containerBuilder.Build();

            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            httpConfiguration.EnsureInitialized();
        }
    }
}