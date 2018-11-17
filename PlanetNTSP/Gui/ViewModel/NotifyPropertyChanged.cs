using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gui.ViewModel
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
