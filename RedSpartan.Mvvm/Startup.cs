using RedSpartan.Mvvm.Core;
using RedSpartan.Mvvm.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RedSpartan.Mvvm
{
    public class Startup
    {
        public static IIoC IoC { get; private set; }
        
        public static async Task InitiliseAsync<TInitiliser, TViewModel>() 
            where TInitiliser : Initiliser 
            where TViewModel : BaseViewModel
        {
            await InitiliseAsync<TInitiliser, TViewModel>(Services.IoC.Current);
        }

        public static async Task InitiliseAsync<TInitiliser, TViewModel>(IIoC ioC) 
            where TInitiliser : Initiliser 
            where TViewModel : BaseViewModel
        {
            IoC = ioC;
            IoC.Register(Activator.CreateInstance<TInitiliser>());

            await InitiliseNavigation<TViewModel>();
        }

        public static async Task InitiliseAsync<TInitiliser, TMaster, TViewModel>()
            where TInitiliser : Initiliser
            where TMaster : NavigationPage
            where TViewModel : BaseViewModel
        {
            await InitiliseAsync<TInitiliser, TMaster, TViewModel>(Services.IoC.Current);
        }

        public static async Task InitiliseAsync<TInitiliser, TMaster, TViewModel>(IIoC ioC)
            where TInitiliser : Initiliser
            where TMaster : NavigationPage
            where TViewModel : BaseViewModel
        {
            IoC = ioC;
            IoC.Register(Activator.CreateInstance<TInitiliser>());

            await InitiliseNavigation<TMaster, TViewModel>();
        }

        private static async Task InitiliseNavigation<TViewModel>()
            where TViewModel : BaseViewModel
        {
            await IoC.Build<INavigationService>().InitialiseAsync<TViewModel>();
        }

        private static async Task InitiliseNavigation<TMaster, TViewModel>()
            where TMaster : NavigationPage
            where TViewModel : BaseViewModel
        {
            await IoC.Build<INavigationService>().InitialiseAsync<TMaster, TViewModel>();
        }
    }
}
