using RedSpartan.Mvvm.Core;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public interface INavigationService
    {
        #region Properties
        Page CurrentMaster { get; }
        #endregion Properties

        #region Initialisation
        Task InitialiseAsync<TViewModel>()
            where TViewModel : BaseViewModel;
        #endregion Initialisation

        #region ViewModel Navigation
        Task NavigateToAsync<TViewModel>(bool removePreviousPage = false, ViewType viewType = ViewType.Default) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync<TViewModel>(object parameter, bool removePreviousPage = false, ViewType viewType = ViewType.Default) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync(Type viewModelType, bool removePreviousPage = false, ViewType viewType = ViewType.Default);

        Task NavigateToAsync(Type viewModelType, object parameter, bool removePreviousPage = false, ViewType viewType = ViewType.Default);
        #endregion ViewModel Navigation

        #region Page Navigation
        Task NavigateToAsync(Page page, bool removePreviousPage = false);

        Task NavigateToAsync(Page page, object parameter, bool removePreviousPage = false);
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
