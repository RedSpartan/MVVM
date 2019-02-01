using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RedSpartan.Mvvm.Core
{
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged, IDisposable
    {
        #region Properties
        [field: NonSerialized]
        public bool IsChanged { get; private set; }
        public DateTime LastUpdated { get; private set; }
        #endregion Properties

        #region Events
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Methods
        /// <summary>
        /// Calls the property change event
        /// </summary>
        /// <param name="propertyName">Name of the property that called the event.</param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set the value of the field
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="field">Property that is having it's value set.</param>
        /// <param name="value">Value to set the property</param>
        /// <param name="onValueChange">Action to run if value has changed</param>
        /// <param name="propertyName">Name of the property being set</param>
        /// <param name="validateValue">Validates value.</param>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        protected virtual bool Set<T>(
            ref T field
            , T value
            , Action onValueChange = null
            , Func<T, T, bool> validateValue = null
            , [CallerMemberName]string propertyName = null)
        {
            //if value didn't change
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            //if value changed but didn't validate
            if (validateValue != null && !validateValue(field, value))
                return false;

            IsChanged = true;
            LastUpdated = DateTime.UtcNow;
            field = value;
            onValueChange?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion Methods

        #region IDisposable Members
        [field: NonSerialized]
        private bool _disposed = false;

        ~ObservableObject()
        {
            OnDispose(false);
        }

        public void Dispose()
        {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    
                }
            }
            _disposed = true;
        }

        #endregion IDisposable Members
    }
}

