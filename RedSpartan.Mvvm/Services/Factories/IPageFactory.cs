using RedSpartan.Mvvm.Core;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public interface IPageFactory
    {
        Task InitilisePage(Page page, object parameter);

        Page CreateAndBindPage<TViewModel>(ViewType viewType = ViewType.Display)
            where TViewModel : BaseViewModel;

        Page CreateAndBindPage(Type viewModelType, ViewType viewType = ViewType.Display);

        Task<Page> CreateBindAndInitilisePageAsync<TViewModel>(ViewType viewType = ViewType.Display)
            where TViewModel : BaseViewModel;

        Task<Page> CreateBindAndInitilisePageAsync<TViewModel>(object parameter, ViewType viewType = ViewType.Display)
            where TViewModel : BaseViewModel;

        Type GetPageTypeForViewModel(Type viewModelType, ViewType viewType);
    }
}
