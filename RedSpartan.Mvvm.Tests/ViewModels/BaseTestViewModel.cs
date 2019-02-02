using RedSpartan.Mvvm.Services;
using System.Threading.Tasks;

namespace RedSpartan.Mvvm.Tests.ViewModels
{
    class BaseTestViewModel : Core.BaseViewModel
    {
        public object Parameter { get; private set; }
        public BaseTestViewModel(INavigationService navigationService) : base(navigationService) { }
        public BaseTestViewModel() : this(null) { }

        public override Task InitialiseAsync(object navigationData)
        {
            Parameter = navigationData;
            return base.InitialiseAsync(navigationData);
        }
    }
}
