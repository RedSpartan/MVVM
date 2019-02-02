using System.Threading.Tasks;
using RedSpartan.Mvvm.Services;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Tests
{
    internal class TestViewModel1 : Core.BaseViewModel
    {
        public object Parameter { get; private set; }
        public TestViewModel1(INavigationService navigationService) : base(navigationService)
        {
        }

        public override Task InitialiseAsync(object navigationData)
        {
            Parameter = navigationData;
            return base.InitialiseAsync(navigationData);
        }
    }

    internal class TestViewModel2 : Core.BaseViewModel
    {
        public object Parameter { get; private set; }

        public TestViewModel2(INavigationService navigationService) : base(navigationService)
        {
        }

        public override Task InitialiseAsync(object navigationData)
        {
            Parameter = navigationData;
            return base.InitialiseAsync(navigationData);
        }
    }
    internal class TestPage1 : Page { }
    internal class TestPage2 : Page { }
    internal class TestInitiliser : Initiliser
    {
        protected override void RegisterServices() { }

        protected override void RegisterViewModelMappings() { }
    }
}
