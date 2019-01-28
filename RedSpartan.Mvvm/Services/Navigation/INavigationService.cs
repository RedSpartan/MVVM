using RedSpartan.Mvvm.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public interface INavigationService
    {
        #region Properties
        Stack<NavigationPage> MasterStack { get; }
        NavigationPage CurrentMaster { get; }
        #endregion Properties

        #region Initialisation
        Task InitialiseAsync<TViewModel>() 
            where TViewModel : BaseViewModel;

        Task InitialiseAsync<TMasterPage, TViewModel>()
            where TMasterPage : NavigationPage
            where TViewModel : BaseViewModel;
        #endregion Initialisation

        #region MasterPage Navigation
        Task PushMasterAsync<TMaster, TViewModel>()
            where TMaster : NavigationPage
            where TViewModel : BaseViewModel;

        Task PushMasterAsync<TMaster, TViewModel>(object navigationData)
            where TMaster : NavigationPage
            where TViewModel : BaseViewModel;

        Task PushMasterAsync(Type typeMaster, Type viewModel);

        Task PushMasterAsync(Type typeMaster, Type viewModel, object navigationData);

        Task PopMasterAsync();
        #endregion MasterPage Navigation

        #region Page Navigation
        Task NavigateToAsync<TViewModel>(bool removePreviousPage = false, ViewType viewType = ViewType.Default) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync<TViewModel>(object parameter, bool removePreviousPage = false, ViewType viewType = ViewType.Default) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync(Type viewModelType, bool removePreviousPage = false, ViewType viewType = ViewType.Default);

        Task NavigateToAsync(Type viewModelType, object parameter, bool removePreviousPage = false, ViewType viewType = ViewType.Default);
        #endregion Page Navigation

        #region Modal Navigation
        Task PushModalAsync<TViewModel>(ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel;

        Task PushModalAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel;

        Task PopModalAsync();
        #endregion Modal Navigation

        Task RemoveBackStackAsync();

        Task RemoveLastFromBackStackAsync();
    }
}
