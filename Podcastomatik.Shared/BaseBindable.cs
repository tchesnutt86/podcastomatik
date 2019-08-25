using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Podcastomatik.Shared
{
    public abstract class BaseBindable : INotifyPropertyChanged
    {
        private bool isWorking;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsWorking
        {
            get => isWorking;
            set
            {
                isWorking = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners. This
        ///     value is optional and can be provided automatically when invoked from compilers
        ///     that support <see cref="CallerMemberNameAttribute" />.
        /// </param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
