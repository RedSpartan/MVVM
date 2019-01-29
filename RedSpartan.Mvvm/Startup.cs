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
        
        /// <summary>
        /// Initialise MVVM with a specific ViewModel using a default NavigationPage
        /// </summary>
        /// <typeparam name="TInitiliser">Initialisation class</typeparam>
        /// <typeparam name="TViewModel">ViewModel to start with</typeparam>
        /// <returns>Asynchronous Task</returns>
        public static async Task InitiliseAsync<TInitiliser, TViewModel>() 
            where TInitiliser : Initiliser 
            where TViewModel : BaseViewModel
        {
            await InitiliseAsync<TInitiliser, TViewModel>(Services.IoC.Current);
        }

        /// <summary>
        /// Initialise MVVM with a specific ViewModel using a default NavigationPage
        /// </summary>
        /// <typeparam name="TInitiliser">Initialisation class</typeparam>
        /// <typeparam name="TViewModel">ViewModel to start with</typeparam>
        /// <param name="ioC">Inversion of Control implementation to use</param>
        /// <returns>Asynchronous Task</returns>
        public static async Task InitiliseAsync<TInitiliser, TViewModel>(IIoC ioC) 
            where TInitiliser : Initiliser 
            where TViewModel : BaseViewModel
        {
            IoC = ioC;
            IoC.Register(Activator.CreateInstance<TInitiliser>());

            //await InitiliseNavigation<TViewModel>();
        }

        private static async Task InitiliseNavigation<TViewModel>()
            where TViewModel : BaseViewModel
        {
            await IoC.Build<INavigationService>().InitialiseAsync<TViewModel>();
        }
    }
}
