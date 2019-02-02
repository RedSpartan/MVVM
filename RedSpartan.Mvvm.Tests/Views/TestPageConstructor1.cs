using RedSpartan.Mvvm.Tests.ViewModels;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Tests.Views
{
    internal class TestPageConstructor1 : Page
    {
        public TestPageConstructor1(TestViewModel1 viewModel)
        {
            BindingContext = viewModel;
        }
    }
}
