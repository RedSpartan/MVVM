using RedSpartan.Mvvm.Core;
using System;
using System.Threading.Tasks;

namespace RedSpartan.Mvvm.Services
{
    public interface INavigationService
    {
        #region Initialisation
        Task InitialiseAsync<TViewModel>(bool includeNavigationPage = true)
            where TViewModel : BaseViewModel;
        #endregion Initialisation

        #region ViewModel Navigation
        Task NavigateToAsync<TViewModel>(BaseViewModel from, ViewType viewType = ViewType.Display, bool removePreviousPage = false) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync<TViewModel>(BaseViewModel from, object parameter, ViewType viewType = ViewType.Display, bool removePreviousPage = false) 
            where TViewModel : BaseViewModel;

        Task NavigateToAsync(BaseViewModel from, Type viewModelType, ViewType viewType = ViewType.Display, bool removePreviousPage = false);

        Task NavigateToAsync(BaseViewModel from, Type viewModelType, object parameter, ViewType viewType = ViewType.Display, bool removePreviousPage = false);
        #endregion ViewModel Navigation

        #region Modal Navigation
        Task PushModalAsync<TViewModel>(ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel;

        Task PushModalAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Display) where TViewModel : BaseViewModel;

        Task PopModalAsync();
        #endregion Modal Navigation
        
        Task RemoveBackStackAsync(BaseViewModel from);

        Task RemoveLastFromBackStackAsync(BaseViewModel from);
    }
}
