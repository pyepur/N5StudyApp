using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace N5StudyApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Added ? to allow this to be null when no one is listening
        public event PropertyChangedEventHandler? PropertyChanged;

        // Added ? to the string so it's allowed to be null initially
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Added ? to propertyName here as well
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}