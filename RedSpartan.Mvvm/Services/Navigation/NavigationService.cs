using RedSpartan.Mvvm.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public class NavigationService : INavigationService
    {
        #region Public Properties
        public NavigationPage CurrentMaster => MasterStack.Peek();

        public Stack<NavigationPage> MasterStack { get; }
        #endregion Public Properties

        #region Protected Properties
        protected Application CurrentApplication => Application.Current;

        protected IViewModelViewMappings Mappings { get; }

        protected IIoC IoC { get; }
        #endregion Protected Properties

        #region Constructors
        /// <summary>
        /// Xamarin ViewModel first navigation service with 
        /// </summary>
        /// <param name="ioC">Inversion of Control implementation</param>
        /// <param name="mappings">ViewModel to View mappings</param>
        public NavigationService(IIoC ioC, IViewModelViewMappings mappings)
        {
            Mappings = mappings;
            IoC = ioC;
            MasterStack = new Stack<NavigationPage>();
        }
        #endregion

        #region INavigationService Implementation
        #region Initialisation
        /// <summary>
        /// Initialise the navigation service by navigating to first page
        /// </summary>
        /// <typeparam name="TViewModel">Page to navigate to</typeparam>
        /// <returns></returns>
        public async Task InitialiseAsync<TViewModel>() where TViewModel : BaseViewModel
        {
            await InitialiseAsync<NavigationPage, TViewModel>();
        }

        /// <summary>
        /// Initialise the navigation service by navigating to first page
        /// </summary>
        /// <typeparam name="TMasterPage">Master Navigation Page</typeparam>
        /// <typeparam name="TViewModel">Page to navigate to</typeparam>
        /// <returns></returns>
        public async Task InitialiseAsync<TMasterPage, TViewModel>()
            where TMasterPage : NavigationPage
            where TViewModel : BaseViewModel
        {
            await InitialiseMasterAsync(typeof(TMasterPage), typeof(TViewModel), null);
        }
        #endregion Initialisation

        #region MasterPage Navigation
        /// <summary>
        /// Pushes a new master page onto the Master stack
        /// </summary>
        /// <typeparam name="TMaster">Type of NavigationPage to display</typeparam>
        /// <typeparam name="TViewModel">ViewModel to add to the Navigation Page</typeparam>
        /// <returns>Asynchronous Task</returns>
        public async Task PushMasterAsync<TMaster, TViewModel>()
            where TMaster : NavigationPage
            where TViewModel : BaseViewModel
        {
            await PushMasterAsync(typeof(TMaster), typeof(TViewModel), null);
        }

        /// <summary>
        /// Pushes a new master page onto the Master stack
        /// </summary>
        /// <typeparam name="TMaster">Type of NavigationPage to display</typeparam>
        /// <typeparam name="TViewModel">ViewModel to add to the Navigation Page</typeparam>
        /// <param name="navigationData">Navigation data to pass to the page</param>
        /// <returns>Asynchronous Task</returns>
        public async Task PushMasterAsync<TMaster, TViewModel>(object navigationData)
            where TMaster : NavigationPage
            where TViewModel : BaseViewModel
        {
            await PushMasterAsync(typeof(TMaster), typeof(TViewModel), navigationData);
        }

        /// <summary>
        /// Pushes a new master page onto the Master stack
        /// </summary>
        /// <param name="typeMaster">Type of NavigationPage to display</param>
        /// <param name="viewModel">ViewModel to add to the Navigation Page</param>
        /// <returns>Asynchronous Task</returns>
        public async Task PushMasterAsync(Type typeMaster, Type viewModel)
        {
            await PushMasterAsync(typeMaster, viewModel, null);
        }

        /// <summary>
        /// Pushes a new master page onto the Master stack
        /// </summary>
        /// <param name="typeMaster">Type of NavigationPage to display</param>
        /// <param name="viewModel">ViewModel to add to the Navigation Page</param>
        /// <param name="navigationData">Navigation data to pass to the page</param>
        /// <returns>Asynchronous Task</returns>
        public async Task PushMasterAsync(Type typeMaster, Type viewModel, object navigationData)
        {
            await InitialiseMasterAsync(typeMaster, viewModel, navigationData);
        }

        public async Task PopMasterAsync()
        {
            if (MasterStack.Count == 1)
                throw new InvalidOperationException("Cannot pop the last Navigation Page");

            await Task.Run(() =>
            {
                MasterStack.Pop();
                CurrentApplication.MainPage = MasterStack.Peek();
            });
        }
        #endregion MasterPage Navigation

        #region Page Navigation
        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel to find a page for</typeparam>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns>Asynchronous Task</returns>
        public async Task NavigateToAsync<TViewModel>(bool removePreviousPage = false, ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel
        {
            await NavigateToAsync(typeof(TViewModel), null, removePreviousPage, viewType);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel to find a page for</typeparam>
        /// <param name="parameter"></param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync<TViewModel>(object parameter, bool removePreviousPage = false, ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel
        {
            await NavigateToAsync(typeof(TViewModel), parameter, removePreviousPage, viewType);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel to find a page for</param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync(Type viewModelType, bool removePreviousPage = false, ViewType viewType = ViewType.Default)
        {
            await NavigateToAsync(viewModelType, null, removePreviousPage, viewType);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel to find a page for</param>
        /// <param name="parameter"></param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync(Type viewModelType, object parameter, bool removePreviousPage = false, ViewType viewType = ViewType.Default)
        {
            Page page = CreateAndBindPage(viewModelType, parameter, viewType);

            Page remove = null;

            if (removePreviousPage)
                remove = GetLastPage();

            await CurrentMaster.PushAsync(page);

            if (removePreviousPage)
                await RemovePageFromBackStackAsync(remove);

            await InitilisePage(page, parameter);
        }
        #endregion Page Navigation

        #region Modal Navigation
        /// <summary>
        /// Pushes Modal page to top
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewType"></param>
        /// <returns>Asynchronous Task</returns>
        public async Task PushModalAsync<TViewModel>(ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel
        {
            await PushModalAsync<TViewModel>(null, viewType);
        }

        /// <summary>
        /// Pushes Modal page to top
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="viewType"></param>
        /// <returns>Asycronious Task</returns>
        public async Task PushModalAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel
        {
            Page page = CreateAndBindPage(typeof(TViewModel), parameter, viewType);

            await CurrentMaster.Navigation.PushModalAsync(page);

            await (page.BindingContext as BaseViewModel).InitialiseAsync(parameter);
        }

        /// <summary>
        /// Pop Modal page from top
        /// </summary>
        /// <returns>Asycronious Task</returns>
        public async Task PopModalAsync()
        {
            await CurrentMaster.Navigation.PopModalAsync();
        }
        #endregion Modal Navigation

        /// <summary>
        /// Removes the whole stack from the current master page
        /// </summary>
        /// <returns>Asynchronous Task</returns>
        public async Task RemoveBackStackAsync()
        {
            while (CurrentMaster.Navigation.NavigationStack.Count > 1)
            {
                await RemovePageAsync(CurrentMaster.Navigation.NavigationStack[0]);
            }
        }

        /// <summary>
        /// Removes the top page from the current master page
        /// </summary>
        /// <returns>Asynchronous Task</returns>
        public async Task RemoveLastFromBackStackAsync()
        {
            await CurrentMaster.PopAsync();
        }
        #endregion INavigationService Implementation

        #region Private Methods
        /// <summary>
        /// Creates a new NavigationPage adding a Page in the constructor with navigation data
        /// </summary>
        /// <param name="typeMaster">NavigationPage to add</param>
        /// <param name="initialPage">Child page to add to the master</param>
        /// <param name="parameter">Parameters to add to child page</param>
        /// <returns>Asynchronous Task</returns>
        private async Task InitialiseMasterAsync(Type typeMaster, Type initialPage, object parameter)
        {
            var page = CreateAndBindPage(initialPage);

            if (!(Activator.CreateInstance(typeMaster, page) is NavigationPage navigationPage))
                throw new InvalidOperationException($"Type {typeMaster.Name} is not a Navigation Page");

            MasterStack.Push(navigationPage);

            CurrentApplication.MainPage = MasterStack.Peek();

            await InitilisePage(page, parameter);
        }

        /// <summary>
        /// Creates a page and binds a ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel type to build</param>
        /// <param name="viewType">Type of view to bind to</param>
        /// <returns>A bound page</returns>
        private Page CreateAndBindPage(Type viewModelType, ViewType viewType = ViewType.Default)
        {
            return CreateAndBindPage(viewModelType, null, viewType);
        }

        /// <summary>
        /// Creates a page and binds a ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel type to build</param>
        /// <param name="parameter">object parameters to add to ViewModel</param>
        /// <param name="viewType">Type of view to bind to</param>
        /// <returns>A bound page</returns>
        private Page CreateAndBindPage(Type viewModelType, object parameter, ViewType viewType = ViewType.Default)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType, viewType);

            if (pageType == null)
                throw new InvalidOperationException($"Mapping type for {viewModelType.Name} is not a page");

            if (!(Activator.CreateInstance(pageType) is Page page))
                throw new InvalidOperationException($"Type {pageType.Name} is not a page");

            page.BindingContext = IoC.Build(viewModelType) as BaseViewModel;

            return page;
        }

        /// <summary>
        /// Returns View type for binding
        /// </summary>
        /// <param name="viewModelType">ViewModel type to get a page for</param>
        /// <param name="viewType">Type of page to get</param>
        /// <returns>A Type</returns>
        private Type GetPageTypeForViewModel(Type viewModelType, ViewType viewType)
        {
            if (Mappings.ContainsKey(viewModelType, viewType))
            {
                return Mappings.GetViewType(viewModelType, viewType);
            }
            else
            {
                var result = GetPageTypeForViewModel(viewModelType);

                if (result != null)
                    return result;
            }

            throw new KeyNotFoundException($"No map for {viewModelType} was found on navigation mappings");
        }

        /// <summary>
        /// Removes a page from the page stack
        /// </summary>
        /// <param name="page">Page to remove</param>
        /// <returns>Asynchronous Task</returns>
        private async Task RemovePageAsync(Page page)
        {
            await Task.Run(() =>
            {
                CurrentMaster.Navigation.RemovePage(page);
            });
        }

        /// <summary>
        /// Page initialisation passing parameters and adding bindings
        /// </summary>
        /// <param name="page">Page to initialise</param>
        /// <param name="parameter">Object Parameter</param>
        /// <returns>Asynchronous Task</returns>
        private async Task InitilisePage(Page page, object parameter)
        {
            page.SetBinding(Page.TitleProperty, "Title", BindingMode.OneWay);
            
            await (page.BindingContext as BaseViewModel).InitialiseAsync(parameter);
        }

        /// <summary>
        /// Looks for a Page in the namespace of the ViewModel with the same name
        /// </summary>
        /// <param name="viewModelType">ViewModel to find a Page for</param>
        /// <returns>A Type</returns>
        private static Type GetPageTypeForViewModel(Type viewModelType)
        {
            //TODO: Looks ugly and needs to be cleaned up
            var viewName = viewModelType.FullName.Replace(".ViewModel", ".View").Replace("ViewModel", "Page");
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            return Type.GetType(viewAssemblyName);
        }

        /// <summary>
        /// Removes a page from the Stack, if no page is passed the last one is removed
        /// </summary>
        /// <param name="page">Page to remove</param>
        /// <returns>Asynchronous Task</returns>
        private async Task RemovePageFromBackStackAsync(Page page = null)
        {
            if (page == null)
                page = GetLastPage();

            await RemovePageAsync(page);
        }

        /// <summary>
        /// Get's the last page from the Stack
        /// </summary>
        /// <returns>A Page</returns>
        private Page GetLastPage()
        {
            var i = CurrentMaster.Navigation.NavigationStack.Count - 1;
            return CurrentMaster.Navigation.NavigationStack[i];
        }
        #endregion Private Methods
    }
}