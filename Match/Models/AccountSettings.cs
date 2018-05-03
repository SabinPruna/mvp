using System.ComponentModel;
using System.Runtime.CompilerServices;
using Match.Annotations;

namespace Match.Models {
    public class AccountSettings : INotifyPropertyChanged {
        private int _gridSizeX;
        private int _gridSizeY;
        private int _time;

        #region Properties

        public int GridSizeX {
            get => _gridSizeX;
            set {
                _gridSizeX = value;
                OnPropertyChanged(nameof(GridSizeX));
            }
        }

        public int GridSizeY {
            get => _gridSizeY;
            set {
                _gridSizeY = value;
                OnPropertyChanged(nameof(GridSizeY));
            }
        }

        public int Time {
            get => _time;
            set {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}