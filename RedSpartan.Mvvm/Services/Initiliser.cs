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
            RegisterViewModelMappings();
            RegisterMappingsToIoC();
        }

        private void RegisterMappingsToIoC()
        {
            foreach (var mapping in Mappings)
            {
                Register.RegisterType(mapping.ViewModel);
                Register.RegisterType(mapping.View);
            }
        }

        protected abstract void RegisterServices();
        
        protected abstract void RegisterViewModelMappings();

        protected virtual void RegisterServicesToContainerBuilder()
        {
            Register
                .RegisterType<NavigationService>()
                .As<INavigationService>()
                .SingleInstance();

            Register
                .RegisterType<PageFactory>()
                .As<IPageFactory>()
                .SingleInstance();

            Register
                .RegisterType<ViewModelViewMappings>()
                .As<IViewModelViewMappings>()
                .SingleInstance();

            Register
                .RegisterType<ViewModelLocator>()
                .As<IViewModelLocator>()
                .SingleInstance();
        }

        internal void RegisterIoC(IIoC ioC)
        {
            Register.RegisterInstance(ioC);
        }
    }
}
