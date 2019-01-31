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
        #region Properties
        public Page CurrentMaster { get; private set; }

        protected Application CurrentApplication => Application.Current;

        protected IViewModelViewMappings Mappings { get; }

        protected IIoC IoC { get; }
        #endregion Properties

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
        }
        #endregion

        #region INavigationService Implementation
        #region Initialisation
        /// <summary>
        /// Initialise the navigation service by navigating to first page
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel to navigate to</typeparam>
        /// <returns></returns>
        public async Task InitialiseAsync<TViewModel>() where TViewModel : BaseViewModel
        {
            var page = CreateAndBindPage(typeof(TViewModel));

            if (page.Implements<NavigationPage>())
            {
                CurrentMaster = page;
            }
            else if (page.Implements<TabbedPage>())
            {
                InitiliseTabbedChildrenAsync((TabbedPage)page);
                CurrentMaster = page;
            }
            else
            {
                CurrentMaster = new NavigationPage(page);
            }
            
            CurrentApplication.MainPage = CurrentMaster;

            await InitilisePage(page, null);
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
        public async Task NavigateToAsync<TViewModel>(ViewType viewType = ViewType.Default, bool removePreviousPage = false) where TViewModel : BaseViewModel
        {
            await NavigateToAsync(typeof(TViewModel), null, viewType, removePreviousPage);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel to find a page for</typeparam>
        /// <param name="parameter"></param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Default, bool removePreviousPage = false) where TViewModel : BaseViewModel
        {
            await NavigateToAsync(typeof(TViewModel), parameter, viewType, removePreviousPage);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel to find a page for</param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync(Type viewModelType, ViewType viewType, bool removePreviousPage = false)
        {
            await NavigateToAsync(viewModelType, null, viewType, removePreviousPage);
        }

        /// <summary>
        /// Navigate to the page for the passed ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel to find a page for</param>
        /// <param name="parameter"></param>
        /// <param name="removePreviousPage">Remove previous page after pushing new one</param>
        /// <param name="viewType">Type of page to find</param>
        /// <returns></returns>
        public async Task NavigateToAsync(Type viewModelType, object parameter, ViewType viewType, bool removePreviousPage = false)
        {
            Page page = CreateAndBindPage(viewModelType, parameter, viewType);
            
            Page remove = null;

            if (removePreviousPage)
                remove = GetLastPage();

            if (CurrentMaster.Implements<NavigationPage>() == false)
            { 
                CurrentMaster = new NavigationPage(CurrentMaster);
            }

            await ((NavigationPage)CurrentMaster).PushAsync(page);

            if (removePreviousPage)
                await RemovePageFromBackStackAsync(remove);

            await InitilisePage(page, parameter);
        }
        #endregion ViewModel Navigation

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

        #region Default Navigation
        public async Task NavigateToAsync(Type type, bool removePreviousPage = false)
        {
            await NavigateToAsync(type, null, removePreviousPage);
        }

        public async Task NavigateToAsync(Type type, object parameter, bool removePreviousPage = false)
        {
            if(type.Implements<BaseViewModel>())
            {
                await NavigateToAsync(type, parameter, ViewType.Default, removePreviousPage);
            }
            else if (type.Implements<Page>())
            {

            }

            throw new InvalidOperationException($"Unknown navigation type {type.Name}");
        }
        #endregion Default Navigation

        #region Page Navigation
        /// <summary>
        /// Bind a page to it's ViewModel
        /// </summary>
        /// <param name="page">Page to find a ViewModel for</param>
        /// <param name="removePreviousPage">Remove previous page from stack</param>
        /// <returns>Asynchronous Task</returns>
        public async Task BindViewModelToPage(Page page)
        {
            await BindViewModelToPage(page, null);
        }

        /// <summary>
        /// Bind a page to it's ViewModel
        /// </summary>
        /// <param name="page">Page to find a ViewModel for</param>
        /// <param name="parameter">ViewModel data for initialisation</param>
        /// <param name="removePreviousPage">Remove previous page from stack</param>
        /// <returns>Asynchronous Task</returns>
        public async Task BindViewModelToPage(Page page, object parameter)
        {
            var type = Mappings.GetDefaultViewModelType(page) ?? GetViewModelTypeForPage(page.GetType());
            page.BindingContext = (BaseViewModel)IoC.Build(type);
            await InitilisePage(page, parameter);
        }
        #endregion Page Navigation

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
            if (CurrentMaster.Implements<NavigationPage>())
                await ((NavigationPage)CurrentMaster).PopAsync();
        }
        #endregion INavigationService Implementation

        #region Private Methods
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

        private static Type GetViewModelTypeForPage(Type pageType)
        {
            //TODO: Looks ugly and needs to be cleaned up
            var pageName = pageType.FullName.Replace(".View", ".ViewModel").Replace("Page", "ViewModel");
            var pageAssemblyName = pageType.GetTypeInfo().Assembly.FullName;
            var viewModelAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", pageName, pageAssemblyName);
            return Type.GetType(viewModelAssemblyName);
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

        /// <summary>
        /// Implements and Initialises ViewModels and their pages from a TabbedPage
        /// </summary>
        /// <param name="page">TabbedPage</param>
        private async void InitiliseTabbedChildrenAsync(TabbedPage page)
        {
            //TODO: running initialise and awaiting is slow and needs refactoring
            if (page.BindingContext is BaseViewModel model && model.Children.Count > 0)
            {
                foreach (var childType in model.Children)
                {
                    var child = CreateAndBindPage(childType);
                    await InitilisePage(child, null);

                    if (child.Implements<TabbedPage>())
                        InitiliseTabbedChildrenAsync((TabbedPage)child);

                    page.Children.Add(child);
                }
            }
        }
        #endregion Private Methods
    }
}