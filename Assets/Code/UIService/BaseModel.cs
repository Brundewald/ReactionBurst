using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MyProject.ReactionBurst.UI
{
    /// <summary>
    /// Base class for all Model for all View
    /// </summary>
    public class BaseModel
    {
        public event Action<string, object> PropertyChanged = delegate {  };

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged(object value, [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(propertyName, value);
        }
        
        protected T CheckPropertyChanged<T>(T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (oldValue.Equals(newValue))
            {
                return oldValue;
            }
            
            OnPropertyChanged(newValue, propertyName);
            return newValue;
        }
    }
}