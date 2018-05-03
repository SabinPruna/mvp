using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MatchGame.Models {
    public class Options : INotifyPropertyChanged {
        private int _columns;
        private int _rows;
        private string _timeLimit;

        #region Properties

        public int Columns {
            get => _columns;
            set {
                if (value == _columns) {
                    return;
                }

                _columns = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(IsStandardSize));
                OnPropertyChanged(nameof(IsNotStandardSize));
            }
        }

        public bool IsNotStandardSize => !IsStandardSize;

        public bool IsStandardSize => Rows == 4 && Columns == 4;

        public int Rows {
            get => _rows;
            set {
                if (value == _rows) {
                    return;
                }

                _rows = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(IsStandardSize));
                OnPropertyChanged(nameof(IsNotStandardSize));
            }
        }

        public string Size => "Custom size" + (IsStandardSize ? "" : $": {Rows} x {Columns}");

        /// <summary>
        ///     Timelimit in seconds
        /// </summary>
        public string TimeLimit {
            get => $"Time limit:{_timeLimit}";
            set {
                if (value == _timeLimit) {
                    return;
                }

                _timeLimit = value.Split(':').LastOrDefault();

                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public Options(string timeLimit, int rows, int columns) {
            TimeLimit = timeLimit;
            Rows = rows;
            Columns = columns;
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