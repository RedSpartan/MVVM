using System;
using System.Collections.Generic;

namespace RedSpartan.Mvvm.Services
{
    public interface IViewModelViewMappings
    {
        int Count { get; }
        void AddMapping(ViewMapping mapping);
        void AddMapping(IEnumerable<ViewMapping> mappings);
        bool ContainsKey(Type viewModel, ViewType viewType = ViewType.Default);
        Type GetViewType(Type viewModel, ViewType viewType = ViewType.Default);
    }
}