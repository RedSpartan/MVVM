using RedSpartan.Mvvm.Core;
using System;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Services
{
    public class ViewMapping
    {
        #region Properties
        /// <summary>
        /// The ViewModel Type
        /// </summary>
        public Type ViewModel { get; protected set; }

        /// <summary>
        /// The View Type
        /// </summary>
        public Type View { get; protected set; }

        /// <summary>
        /// The type of view
        /// </summary>
        public ViewType ViewType { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// A mapping of a ViewModel to a View
        /// </summary>
        /// <param name="viewModel">Type of ViewModel to map from</param>
        /// <param name="view">Type of View to Map to</param>
        /// <param name="viewType">The type of mapping to be used</param>
        internal ViewMapping(Type viewModel, Type view, ViewType viewType = ViewType.Display)
        {
            ViewModel = viewModel;
            View = view;
            ViewType = viewType;
        }
        #endregion

        #region Equality Override
        private bool Equals(ViewMapping other)
        {
            return ViewModel.Equals(other.ViewModel)
                    && ViewType.Equals(other.ViewType);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != GetType()) return false;

            return Equals((ViewMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ ViewModel.GetHashCode();
                hash = (hash * 16777619) ^ ViewType.GetHashCode();
                return hash;
            }
        }
        #endregion Equality Override
    }

    public sealed class ViewMapping<TViewModel, TView> : ViewMapping
        where TViewModel : BaseViewModel
        where TView : Page
    {
        #region Constructor
        /// <summary>
        /// A mapping of a ViewModel to a View
        /// </summary>
        /// <param name="viewModel">Type of ViewModel to map from</param>
        /// <param name="view">Type of View to Map to</param>
        /// <param name="viewType">The type of mapping to be used</param>
        public ViewMapping(ViewType viewType = ViewType.Display) :
            base(typeof(TViewModel), typeof(TView), viewType)
        { }
        #endregion
    }
}
