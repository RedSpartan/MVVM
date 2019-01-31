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
        Task NavigateToAsync<TViewModel>(ViewType viewType = ViewType.Default, bool removePreviousPage = false) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Default, bool removePreviousPage = false) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync(Type viewModelType, ViewType viewType, bool removePreviousPage = false);

        Task NavigateToAsync(Type viewModelType, object parameter, ViewType viewType, bool removePreviousPage = false);
        #endregion ViewModel Navigation

        #region Modal Navigation
        Task PushModalAsync<TViewModel>(ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel;

        Task PushModalAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Default) where TViewModel : BaseViewModel;

        Task PopModalAsync();
        #endregion Modal Navigation
        
        Task RemoveBackStackAsync();

        Task RemoveLastFromBackStackAsync();
    }
}
