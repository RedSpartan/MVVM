using RedSpartan.Mvvm.Core;
using RedSpartan.Mvvm.Services;
using System;
using System.Threading.Tasks;

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
        public static async Task InitiliseAsync<TInitiliser, TViewModel>(bool includeNavigationPage = true) 
            where TInitiliser : Initiliser 
            where TViewModel : BaseViewModel
        {
            await InitiliseAsync<TInitiliser, TViewModel>(Services.IoC.Instance, includeNavigationPage);
        }

        /// <summary>
        /// Initialise MVVM with a specific ViewModel using a default NavigationPage
        /// </summary>
        /// <typeparam name="TInitiliser">Initialisation class</typeparam>
        /// <typeparam name="TViewModel">ViewModel to start with</typeparam>
        /// <param name="ioC">Inversion of Control implementation to use</param>
        /// <returns>Asynchronous Task</returns>
        public static async Task InitiliseAsync<TInitiliser, TViewModel>(IIoC ioC, bool includeNavigationPage = true) 
            where TInitiliser : Initiliser 
            where TViewModel : BaseViewModel
        {
            await InitiliseAsync<TInitiliser>(ioC, includeNavigationPage);
            await InitiliseNavigation<TViewModel>(includeNavigationPage);
        }

        /// <summary>
        /// Initialise MVVM Mappings but does not load a default page
        /// </summary>
        /// <typeparam name="TInitiliser">Initialisation class</typeparam>
        /// <returns>Asynchronous Task</returns>
        public static async Task InitiliseAsync<TInitiliser>(bool includeNavigationPage = true)
            where TInitiliser : Initiliser
        {
            await InitiliseAsync<TInitiliser>(Services.IoC.Instance, includeNavigationPage);
        }

        /// <summary>
        /// Initialise MVVM Mappings but does not load a default page
        /// </summary>
        /// <typeparam name="TInitiliser">Initialisation class</typeparam>
        /// <param name="ioC">Inversion of Control implementation to use</param>
        /// <returns>Asynchronous Task</returns>
        public static async Task InitiliseAsync<TInitiliser>(IIoC ioC, bool includeNavigationPage = true)
            where TInitiliser : Initiliser
        {
            IoC = ioC;
            IoC.Register(Activator.CreateInstance<TInitiliser>());
        }

        private static async Task InitiliseNavigation<TViewModel>(bool includeNavigationPage = true)
            where TViewModel : BaseViewModel
        {
            await IoC.Build<INavigationService>().InitialiseAsync<TViewModel>(includeNavigationPage);
        }
    }
}
