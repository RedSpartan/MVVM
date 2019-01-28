using Autofac;
using System;
using System.Collections.Generic;

namespace RedSpartan.Mvvm.Services
{
    public abstract class Initiliser
    {
        protected ContainerBuilder Register { get; } = new ContainerBuilder();
        public IList<ViewMapping> Mappings { get; } = new List<ViewMapping>();

        public virtual IContainer BuildRegister()
        {
            RegisterTypes();
            Register.RegisterInstance(this);
            return Register.Build();
        }

        protected void RegisterTypes()
        {
            RegisterServicesToContainerBuilder();
            RegisterServices();
            RegisterViewModels();
            RegisterViewModelMappings();
        }

        protected abstract void RegisterServices();

        protected abstract void RegisterViewModels();

        protected abstract void RegisterViewModelMappings();

        protected virtual void RegisterServicesToContainerBuilder()
        {
            Register
                .RegisterType<NavigationService>()
                .As<INavigationService>()
                .SingleInstance();

            Register
                .RegisterType<ViewModelViewMappings>()
                .As<IViewModelViewMappings>()
                .SingleInstance();
        }

        internal void RegisterIoC(IIoC ioC)
        {
            Register.RegisterInstance(ioC);
        }
    }
}
