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
        bool ContainsKey(Type viewModel, ViewType viewType = ViewType.Default);
        Type GetViewType(Type viewModel, ViewType viewType = ViewType.Default);
        ViewMapping GetMapping(Type viewModel, ViewType viewType = ViewType.Default);
        Type GetDefaultViewModelType(Page page);
    }
}