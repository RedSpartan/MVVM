using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RedSpartan.Mvvm.Services
{
    public sealed class IoC : IIoC
    {
        #region Fields
        private IContainer _container;
        #endregion Fields

        #region Singleton
        private static readonly Lazy<IoC> _lazy = new Lazy<IoC>(() => new IoC());
        public static IoC Instance => _lazy.Value;
        #endregion Singleton

        #region Constructor
        private IoC() { }
        #endregion Constructor

        #region IIoC Methods
        public T Build<T>()
        {
            return _container.Resolve<T>();
        }

        public object Build(Type type)
        {
            return _container.Resolve(type);
        }

        public async Task<T> BuildAsync<T>()
        {
            return await Task.FromResult(_container.Resolve<T>());
        }

        public async Task<object> BuildAsync(Type type)
        {
            return await Task.FromResult(_container.Resolve(type));
        }

        public void Register(Initiliser initiliser)
        {
            initiliser.RegisterIoC(this);
            if (_container is null)
                _container = initiliser.BuildRegister();
            else
                throw new InvalidOperationException("Register has already been called and cannot be called a second time.");
        }
        #endregion IIoC Methods
    }
}
