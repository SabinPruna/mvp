using System.ComponentModel;
using System.Runtime.CompilerServices;
using Match.Annotations;

namespace Match.Models {
    public class Account : INotifyPropertyChanged {
        private AccountSettings _accountSettings;
        private string _imagePath;
        private string _username;

        #region Properties

        public AccountSettings AccountSettings {
            get => _accountSettings;
            set {
                _accountSettings = value;
                OnPropertyChanged(nameof(AccountSettings));
            }
        }

        public int GamesLost { get; set; }
        public int GamesWon { get; set; }

        public string ImagePath {
            get => _imagePath;
            set {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public string Username {
            get => _username;
            set {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        #endregion

        #region Constructors

        public Account() {
            _accountSettings = new AccountSettings();
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