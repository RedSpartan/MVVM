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
            await InitiliseAsync<TInitiliser, TViewModel>(Services.IoC.Instance);
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
            await InitiliseAsync<TInitiliser>(ioC);
            await InitiliseNavigation<TViewModel>();
        }

        /// <summary>
        /// Initialise MVVM Mappings but does not load a default page
        /// </summary>
        /// <typeparam name="TInitiliser">Initialisation class</typeparam>
        /// <returns>Asynchronous Task</returns>
        public static async Task InitiliseAsync<TInitiliser>()
            where TInitiliser : Initiliser
        {
            await InitiliseAsync<TInitiliser>(Services.IoC.Instance);
        }

        /// <summary>
        /// Initialise MVVM Mappings but does not load a default page
        /// </summary>
        /// <typeparam name="TInitiliser">Initialisation class</typeparam>
        /// <param name="ioC">Inversion of Control implementation to use</param>
        /// <returns>Asynchronous Task</returns>
        public static async Task InitiliseAsync<TInitiliser>(IIoC ioC)
            where TInitiliser : Initiliser
        {
            IoC = ioC;
            IoC.Register(Activator.CreateInstance<TInitiliser>());
        }

        private static async Task InitiliseNavigation<TViewModel>()
            where TViewModel : BaseViewModel
        {
            await IoC.Build<INavigationService>().InitialiseAsync<TViewModel>();
        }
    }
}
