using RedSpartan.Mvvm.Services;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Tests
{
    class TestViewModel1 : Core.BaseViewModel { }
    class TestViewModel2 : Core.BaseViewModel { }
    class TestPage1 : Page { }
    class TestPage2 : Page { }
    class TestInitiliser : Initiliser
    {
        protected override void RegisterServices() { }

        protected override void RegisterViewModelMappings() { }
    }
}
