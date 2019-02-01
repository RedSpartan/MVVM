using RedSpartan.Mvvm.Core;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public interface IViewModelViewMappings
    {
        int Count { get; }

        void AddMapping(ViewMapping mapping);

        void AddMapping(IEnumerable<ViewMapping> mappings);

        bool ContainsKey(Type viewModelType, ViewType viewType = ViewType.Display);

        bool ContainsKey<TViewModel>(ViewType viewType = ViewType.Display)
            where TViewModel : BaseViewModel;

        Type GetViewType(Type viewModelType, ViewType viewType = ViewType.Display);

        Type GetViewType<TViewModel>(ViewType viewType = ViewType.Display)
            where TViewModel : BaseViewModel;

        ViewMapping GetMapping<TViewModel>(ViewType viewType = ViewType.Display)
            where TViewModel : BaseViewModel;

        Type GetDefaultViewModelType(Page page);
    }
}