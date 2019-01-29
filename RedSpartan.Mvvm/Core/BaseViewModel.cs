﻿using RedSpartan.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedSpartan.Mvvm.Core
{
    public abstract class BaseViewModel : ObservableObject
    {
        #region Fields
        private bool _isBusy = false;
        private string _title = string.Empty;
        private bool _isNotBusy = true;
        private string _footer = string.Empty;
        private string _header = string.Empty;

        #endregion Fields

        #region Constructors
        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            _title = GetTitle();
        }
        #endregion Constructors

        #region Service Properties
        public INavigationService NavigationService { get; }

        public IList<Type> Children { get; } = new List<Type>();
        #endregion Service Properties

        #region Bindable Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (Set(ref _isBusy, value))
                    IsNotBusy = !IsBusy;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is not busy.
        /// </summary>
        /// <value><c>true</c> if this instance is not busy; otherwise, <c>false</c>.</value>
        public bool IsNotBusy
        {
            get => _isNotBusy;
            set
            {
                if (Set(ref _isNotBusy, value))
                    IsBusy = !_isNotBusy;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get => _title;
            set { Set(ref _title, value); }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header
        {
            get => _header;
            set => Set(ref _header, value);
        }

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        /// <value>The footer.</value>
        public string Footer
        {
            get => _footer;
            set => Set(ref _footer, value);
        }
        #endregion Bindable Properties

        #region Command Properties

        #endregion Command Properties

        #region Methods
        public abstract Task InitialiseAsync(object navigationData);
        
        public virtual string GetTitle()
        {
            return GetType().Name.Replace("ViewModel", "").ToCamelCase();
        }
        #endregion Methods
    }
}
