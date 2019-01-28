using System;
using System.Collections.Generic;

namespace RedSpartan.Mvvm.Services
{
    public class ViewModelViewMappings : IViewModelViewMappings
    {
        #region Properties
        internal IDictionary<int, ViewMapping> Mappings { get; }
        public int Count => Mappings.Count;
        #endregion Properties

        #region Constructors
        /// <summary>
        /// ViewModel to View mappings
        /// </summary>
        /// <param name="initiliser"></param>
        public ViewModelViewMappings(Initiliser initiliser)
        {
            Mappings = new Dictionary<int, ViewMapping>();
            AddMapping(initiliser.Mappings);
        }
        #endregion Constructors

        #region Methods
        public void AddMapping(ViewMapping mapping)
        {
            if (Mappings.Keys.Contains(mapping.GetHashCode()))
            {
                var sb = new System.Text.StringBuilder("ViewMapping Key already exists for type [");
                sb.Append(mapping.ViewModel);
                sb.Append("] with '");
                sb.Append(mapping.ViewType);
                sb.Append("' viewType");
                throw new InvalidOperationException(sb.ToString());
            }
            
            Mappings.Add(mapping.GetHashCode(), mapping);
        }

        public void AddMapping(IEnumerable<ViewMapping> mappings)
        {
            if (mappings is null) return;
            foreach (var mapping in mappings)
                AddMapping(mapping);
        }

        public bool ContainsKey(Type viewModel, ViewType viewType = ViewType.Default)
        {
            return Mappings.Keys.Contains(new ViewMapping(viewModel, null, viewType).GetHashCode());
        }

        public Type GetViewType(Type viewModel, ViewType viewType = ViewType.Default)
        {
            var viewModelMapping = new ViewMapping(viewModel, null, viewType);
            if (Mappings.Keys.Contains(viewModelMapping.GetHashCode()))
            {
                return Mappings[viewModelMapping.GetHashCode()].View;
            }

            var sb = new System.Text.StringBuilder("ViewModel [");
            sb.Append(viewModelMapping.ViewModel);
            sb.Append("] was not found for ViewType ");
            sb.Append(viewModelMapping.ViewType);

            throw new KeyNotFoundException(sb.ToString());
        }
        #endregion Methods
    }
}
