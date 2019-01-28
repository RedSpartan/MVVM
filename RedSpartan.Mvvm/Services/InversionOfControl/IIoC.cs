using System;
using System.Threading.Tasks;

namespace RedSpartan.Mvvm.Services
{
    public interface IIoC
    {
        T Build<T>();

        object Build(Type type);

        Task<T> BuildAsync<T>();

        Task<object> BuildAsync(Type type);

        void Register(Initiliser builder);
    }
}
