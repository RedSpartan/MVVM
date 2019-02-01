using RedSpartan.Mvvm.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public class PageFactory : IPageFactory
    {
        #region Properties
        public IIoC IoC { get; }

        public IViewModelViewMappings Mappings { get; }
        #endregion

        #region Constructors
        public PageFactory(IIoC ioC, IViewModelViewMappings mappings)
        {
            Mappings = mappings;
            IoC = ioC;
        }
        #endregion

        #region IPageFactory Methods
        public Page CreateAndBindPage<TViewModel>(ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel
        {
            return CreateAndBindPage(typeof(TViewModel), null, viewType);
        }

        public Page CreateAndBindPage<TViewModel>(object parameter, ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel
        {
            return CreateAndBindPage(typeof(TViewModel), null, viewType);
        }

        /// <summary>
        /// Creates a page and binds a ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel type to build</param>
        /// <param name="viewType">Type of view to bind to</param>
        /// <returns>A bound page</returns>
        public Page CreateAndBindPage(Type viewModelType, ViewType viewType = ViewType.Display)
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
        public Page CreateAndBindPage(Type viewModelType, object parameter, ViewType viewType = ViewType.Display)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType, viewType);

            if (pageType == null)
                throw new InvalidOperationException($"Mapping type for {viewModelType.Name} is not a page");

            if (!(IoC.Build(pageType) is Page page))
                throw new InvalidOperationException($"Type {pageType.Name} is not a page");

            if(page.BindingContext?.GetType() != viewModelType)
                page.BindingContext = IoC.Build(viewModelType) as BaseViewModel;

            (page.BindingContext as BaseViewModel).CurrentPage = page;

            return page;
        }

        /// <summary>
        /// Uses reflection to find Views based on the name of the ViewModel
        /// </summary>
        /// <param name="viewModelType">ViewModel type to get a page for</param>
        /// <param name="viewType">Type of page to get</param>
        /// <returns>A Type</returns>
        public Type GetPageTypeForViewModel(Type viewModelType, ViewType viewType)
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
        /// Page initialisation passing parameters and adding bindings
        /// </summary>
        /// <param name="page">Page to initialise</param>
        /// <param name="parameter">Object Parameter</param>
        /// <returns>Asynchronous Task</returns>
        public async Task InitilisePage(Page page, object parameter)
        {
            page.SetBinding(Page.TitleProperty, "Title", BindingMode.OneWay);

            await (page.BindingContext as BaseViewModel).InitialiseAsync(parameter);
        }

        public async Task<Page> CreateBindAndInitilisePageAsync<TViewModel>(ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel
        {
            return await CreateBindAndInitilisePageAsync<TViewModel>(null, viewType);
        }

        public async Task<Page> CreateBindAndInitilisePageAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel
        {
            var page = CreateAndBindPage<TViewModel>(null, viewType);

            await InitilisePage(page, parameter);

            return page;
        }
        #endregion

        #region Static Methods
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
        #endregion

        //TODO: Move this into it's own ViewModel Factory
        #region ViewModel Construction
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
        /// Uses reflection to find ViewModelss based on the name of the View or Page
        /// </summary>
        /// <param name="pageType"></param>
        /// <returns></returns>
        private static Type GetViewModelTypeForPage(Type pageType)
        {
            //TODO: Looks ugly and needs to be cleaned up
            var pageName = pageType.FullName.Replace(".View", ".ViewModel").Replace("Page", "ViewModel");
            var pageAssemblyName = pageType.GetTypeInfo().Assembly.FullName;
            var viewModelAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", pageName, pageAssemblyName);
            return Type.GetType(viewModelAssemblyName);
        }
        #endregion
    }
}
