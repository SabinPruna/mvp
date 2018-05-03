using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MatchGame.Models {
    public class SaveGame : INotifyPropertyChanged {
        private string _saveGamePath;

        #region Properties

        public string SaveGamePath {
            get => _saveGamePath.Split('\\').LastOrDefault() ?? "";
            set {
                if (value == _saveGamePath) {
                    return;
                }

                _saveGamePath = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public SaveGame(string saveGamePath) {
            SaveGamePath = saveGamePath;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public override string ToString() {
            return SaveGamePath;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}