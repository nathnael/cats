﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cats.Data.UnitWork;
using Ninject;
using Cats.Services.EarlyWarning;

namespace Cats.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver()
        {
            this.kernel = new StandardKernel();
            AddBindings();
        }

       

    public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        
        private void AddBindings()
        {
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IRegionalRequestService>().To<RegionalRequestService>();
            kernel.Bind<IFDPService>().To<FDPService>();
            kernel.Bind<IAdminUnitService>().To<AdminUnitService>();
            kernel.Bind<IProgramService>().To<ProgramService>();
            kernel.Bind<ICommodityService>().To<CommodityService>();
            kernel.Bind<IRegionalRequestDetailService>().To<RegionalRequestDetailService>();
            kernel.Bind<IReliefRequisitionService>().To<ReliefRequisitionService>();
            kernel.Bind<IReliefRequisitionDetailService>().To<ReliefRequisitionDetailService>();
        }
    }
}