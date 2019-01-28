using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RedSpartan.Mvvm.Services
{
    public class IoC : IIoC
    {
        #region Fields
        private IContainer _container;
        #endregion Fields

        #region Singleton
        private static readonly object _lock = new object();
        private static IoC _current;
        public static IoC Current
        {
            get
            {
                if (_current == null)
                    lock (_lock)
                        if (_current == null)
                            _current = new IoC();

                return _current;
            }
        }
        #endregion Singleton

        #region Constructor
        protected IoC()
        {
            
        }
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
