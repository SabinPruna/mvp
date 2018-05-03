using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MatchGame.Commands;
using MatchGame.Models;
using MatchGame.Views;

namespace MatchGame.ViewModels {
    public class OptionsViewModel : INotifyPropertyChanged {
        private Options _options;

        #region Properties

        public Options Options {
            get => _options;
            set {
                if (Equals(value, _options)) {
                    return;
                }

                _options = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructors

        public OptionsViewModel() {
            Options = new Options("60 seconds", 3, 3);

            SaveCommand = new RelayCommand(
                () => { Application.Current.Windows.OfType<OptionsWindow>().First().Close(); }, () => {
                                                                                                    if (Options == null
                                                                                                    ) {
                                                                                                        return false;
                                                                                                    }

                                                                                                    return
                                                                                                        Options.Rows *
                                                                                                        Options
                                                                                                           .Columns >=
                                                                                                        4;
                                                                                                });
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}