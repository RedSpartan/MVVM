using RedSpartan.Mvvm.Core;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public class NavigationService : INavigationService
    {
        #region Properties
        
        protected Application CurrentApplication => Application.Current;
        
        public IPageFactory PageFactory { get; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Xamarin ViewModel first navigation service with 
        /// </summary>
        /// <param name="ioC">Inversion of Control implementation</param>
        /// <param name="mappings">ViewModel to View mappings</param>
        public NavigationService(IPageFactory pageFactory)
        {
            PageFactory = pageFactory;
        }
        #endregion

        #region INavigationService Implementation
        #region Initialisation
        /// <summary>
        /// Initialise the navigation service by navigating to first page
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel to navigate to</typeparam>
        /// <returns></returns>
        public async Task InitialiseAsync<TViewModel>(bool includeNavigationPage = true) 
            where TViewModel : BaseViewModel
        {
            var page = PageFactory.CreateAndBindPage(typeof(TViewModel));

            if (page.Implements<NavigationPage>())
            {
                CurrentApplication.MainPage = page;
            }
            else if (includeNavigationPage)
            {
                CurrentApplication.MainPage = new NavigationPage(page);
            }
            else
            {
                CurrentApplication.MainPage = page;
            }

            await PageFactory.InitilisePage(page, null);
        }
        #endregion Initialisation

        #region ViewModel Navigation
        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel to find a page for</typeparam>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns>Asynchronous Task</returns>
        public async Task NavigateToAsync<TViewModel>(BaseViewModel from, ViewType viewType = ViewType.Display, bool removePreviousPage = false) where TViewModel : BaseViewModel
        {
            await NavigateToAsync(from, typeof(TViewModel), null, viewType, removePreviousPage);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel to find a page for</typeparam>
        /// <param name="parameter"></param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync<TViewModel>(BaseViewModel from, object parameter, ViewType viewType = ViewType.Display, bool removePreviousPage = false) where TViewModel : BaseViewModel
        {
            await NavigateToAsync(from, typeof(TViewModel), parameter, viewType, removePreviousPage);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel to find a page for</param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync(BaseViewModel from, Type viewModelType, ViewType viewType = ViewType.Display, bool removePreviousPage = false)
        {
            await NavigateToAsync(from, viewModelType, null, viewType, removePreviousPage);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel to find a page for</param>
        /// <param name="parameter"></param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync(BaseViewModel from, Type viewModelType, object parameter, ViewType viewType = ViewType.Display, bool removePreviousPage = false)
        {
            Page page = PageFactory.CreateAndBindPage(viewModelType, viewType);
            
            Page remove = null;

            if (removePreviousPage)
                remove = GetLastPage(from);

            await from.CurrentPage.Navigation.PushAsync(page);

            if (removePreviousPage)
                await RemovePageFromBackStackAsync(from, remove);

            await PageFactory.InitilisePage(page, parameter);
        }
        #endregion ViewModel Navigation

        #region Modal Navigation
        /// <summary>
        /// Pushes Modal page to top
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewType"></param>
        /// <returns>Asynchronous Task</returns>
        public async Task PushModalAsync<TViewModel>(ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel
        {
            await PushModalAsync<TViewModel>(null, viewType);
        }

        /// <summary>
        /// Pushes Modal page to top
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="viewType"></param>
        /// <returns>Asynchronous Task</returns>
        public async Task PushModalAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel
        {
            Page page = PageFactory.CreateAndBindPage(typeof(TViewModel), viewType);

            await CurrentApplication.MainPage.Navigation.PushModalAsync(page);

            await (page.BindingContext as BaseViewModel).InitialiseAsync(parameter);
        }

        /// <summary>
        /// Pop Modal page from top
        /// </summary>
        /// <returns>Asynchronous Task</returns>
        public async Task PopModalAsync()
        {
            await CurrentApplication.MainPage.Navigation.PopModalAsync();
        }
        #endregion Modal Navigation
        
        /// <summary>
        /// Removes the whole stack from the current master page
        /// </summary>
        /// <returns>Asynchronous Task</returns>
        public async Task RemoveBackStackAsync(BaseViewModel from)
        {
            while (from.CurrentPage.Navigation.NavigationStack.Count > 1)
            {
                await RemovePageAsync(from.CurrentPage.Navigation.NavigationStack[0]);
            }
        }

        /// <summary>
        /// Removes the top page from the current master page
        /// </summary>
        /// <returns>Asynchronous Task</returns>
        public async Task RemoveLastFromBackStackAsync(BaseViewModel from)
        {
            //if (CurrentMaster.Implements<NavigationPage>())
                await from.CurrentPage.Navigation.PopAsync();
        }
        #endregion INavigationService Implementation

        #region Private Methods
        /// <summary>
        /// Removes a page from the page stack
        /// </summary>
        /// <param name="page">Page to remove</param>
        /// <returns>Asynchronous Task</returns>
        private async Task RemovePageAsync(Page page)
        {
            await Task.Run(() =>
            {
                page.Navigation.RemovePage(page);
            });
        }

        /// <summary>
        /// Removes a page from the Stack, if no page is passed the last one is removed
        /// </summary>
        /// <param name="page">Page to remove</param>
        /// <returns>Asynchronous Task</returns>
        private async Task RemovePageFromBackStackAsync(BaseViewModel from, Page page = null)
        {
            if (page == null)
                page = GetLastPage(from);

            await RemovePageAsync(page);
        }

        /// <summary>
        /// Get's the last page from the Stack
        /// </summary>
        /// <returns>A Page</returns>
        private Page GetLastPage(BaseViewModel from)
        {
            var i = from.CurrentPage.Navigation.NavigationStack.Count - 1;
            return from.CurrentPage.Navigation.NavigationStack[i];
        }
        #endregion Private Methods
    }
}