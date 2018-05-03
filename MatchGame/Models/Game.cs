using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using MatchGame.Enums;
using MatchGame.Helpers;

namespace MatchGame.Models {
    public class Game : INotifyPropertyChanged {
        private ObservableCollection<Card> _cards;
        private int _roundsWon;
        private int _timeLeft;

        #region Properties

        public Account Account { get; set; }


        public ObservableCollection<Card> Cards {
            get => _cards;
            set {
                if (Equals(value, _cards)) {
                    return;
                }

                _cards = value;
                OnPropertyChanged();
            }
        }


        public Options Options { get; set; }

        public string RoundsWon {
            get => $"Rounds won:{_roundsWon}/3";
            set {
                if (value == $"Rounds won:{_roundsWon}/3") {
                    return;
                }

                char[] digits = value.SkipWhile(c => !char.IsDigit(c))
                                     .TakeWhile(char.IsDigit)
                                     .ToArray();

                string str = new string(digits);
                _roundsWon = int.Parse(str);

                OnPropertyChanged();
            }
        }

        public string TimeLeft {
            get => $"Time left:{_timeLeft}";
            set {
                if (value == $"Time left:{_timeLeft}") {
                    return;
                }

                _timeLeft = int.Parse(value.Split(':').Last());
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public Game() {
            Options = new Options("60 seconds", 4, 4);
            TimeLeft = new string(Options.TimeLimit.Where(char.IsDigit).ToArray());
            RoundsWon = 0.ToString();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartGame() {
            Application.Current.Dispatcher.Invoke(() => {
                                                      Cards = new ObservableCollection<Card>();
                                                      for (int i = 0;
                                                           i < Options.Rows * Options.Columns / 2;
                                                           i++) {
                                                          BitmapImage bitmapImage = Cards.GetRandomCard();
                                                          Cards.Add(new Card(bitmapImage));
                                                          Cards.Add(new Card(bitmapImage));
                                                      }

                                                      Cards.Shuffle();
                                                      Cards.ToList().ForEach(c => c.CardStatus = CardStatusEnum.Normal);
                                                      TimeLeft = new string(
                                                          Options.TimeLimit.Where(char.IsDigit).ToArray());
                                                  });
        }
    }
}