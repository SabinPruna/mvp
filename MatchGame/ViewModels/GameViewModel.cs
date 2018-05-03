using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MatchGame.Commands;
using MatchGame.Enums;
using MatchGame.Helpers;
using MatchGame.Models;
using MatchGame.Views;

namespace MatchGame.ViewModels {
    public class GameViewModel : INotifyPropertyChanged {
        private Game _game;
        private int _listWidth;
        private bool _paused = true;
        private ObservableCollection<SaveGame> _saveGames;

        private Task _updateTimeTask;

        #region Properties

        public ICommand CardFlippedCommand { get; set; }
        public ICommand EditOptionsCommand { get; set; }

        public Game Game {
            get => _game;
            set {
                if (Equals(value, _game)) {
                    return;
                }

                _game = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeLimit));
            }
        }

        public GameStateEnum GameState { get; set; }

        public List<Card> LastSelectedCards { get; set; }

        public int ListWidth {
            get => _listWidth;
            set {
                if (value == _listWidth) {
                    return;
                }

                _listWidth = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadGameCommand { get; set; }
        public ICommand LoadGamePathsCommand { get; set; }
        public ICommand NewGameCommand { get; set; }
        public ICommand SaveGameCommand { get; set; }


        public ObservableCollection<SaveGame> SaveGames {
            get => _saveGames;
            set {
                if (Equals(value, _saveGames)) {
                    return;
                }

                _saveGames = value;
                OnPropertyChanged();
            }
        }

        public Card SelectedCard { get; set; }
        public ICommand StatisticsCommand { get; set; }


        public string TimeLimit {
            get => $"Time limit :{Game.Options.TimeLimit}";
            set {
                if (value == Game.Options.TimeLimit) {
                    return;
                }

                Game.Options.TimeLimit = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public GameViewModel() {
            SelectedCard = null;
            LastSelectedCards = new List<Card>();
            Game = new Game();
            GameState = GameStateEnum.NoGameCurrently;
            _updateTimeTask = Task.Run(() => UpdateTime());

            async void FlipCards(CardStatusEnum cardStatus) {
                Thread.Sleep(cardStatus == CardStatusEnum.Normal ? 300 : 150);
                /*
                                LastSelectedCards.Remove(SelectedCard);
                */


                LastSelectedCards.ForEach(c => c.CardStatus = cardStatus);
                LastSelectedCards.Clear();
                /*
                                LastSelectedCards.Add(SelectedCard);
                */
                if (cardStatus == CardStatusEnum.Normal) {
                    LastSelectedCards.Add(SelectedCard);
                    SelectedCard.CardStatus = CardStatusEnum.Flipped;
                }
                else {
                    if (Game.Cards.All(c => c.CardStatus == CardStatusEnum.Removed)) {
                        int roundsWon = int.Parse(Game.RoundsWon.First(c => char.IsDigit(c)).ToString()) + 1;
                        Game.RoundsWon = roundsWon.ToString();

                        string message;
                        string question;
                        if (roundsWon == 4) {
                            message = "Game won";
                            question = "Start another?";

                            Game.RoundsWon = 0.ToString();
                            Game.Account.GamesWon++;
                            Game.Account.Update();
                        }
                        else {
                            message = "Round won";
                            question = "Start next?";
                        }

                        MessageDialogResult dialogResult = MessageDialogResult.Canceled;
                        await Application.Current.Dispatcher.Invoke(async () => {
                                                                        MetroWindow metroWindow =
                                                                            Application.Current.Windows
                                                                                       .OfType<GameWindow>().First();
                                                                        dialogResult =
                                                                            await metroWindow.ShowMessageAsync(message,
                                                                                                               question,
                                                                                                               MessageDialogStyle
                                                                                                                  .AffirmativeAndNegative);
                                                                    });
                        if (dialogResult == MessageDialogResult.Affirmative) {
                            Game.StartGame();
                        }
                    }
                }
            }

            CardFlippedCommand = new RelayCommand(() => {
                                                      if (SelectedCard == null) {
                                                          return;
                                                      }

                                                      if (SelectedCard.CardStatus == CardStatusEnum.Removed) {
                                                          return;
                                                      }

                                                      LastSelectedCards.Add(SelectedCard);
                                                      SelectedCard.CardStatus = CardStatusEnum.Selected;

                                                      if (LastSelectedCards.Count == 2) {
                                                          LastSelectedCards.ForEach(
                                                              c => c.CardStatus = CardStatusEnum.Flipped);
                                                          if (LastSelectedCards
                                                             .GroupBy(o => o.BitmapImage.UriSource)
                                                             .Select(g => g.First()).Count() !=
                                                              LastSelectedCards.Count) {
                                                              Task.Run(() => FlipCards(CardStatusEnum.Removed));
                                                          }
                                                      }

                                                      if (LastSelectedCards.Count == 3) {
                                                          Task.Run(() => FlipCards(CardStatusEnum.Normal));
                                                          SelectedCard.CardStatus = CardStatusEnum.Flipped;
                                                      }
                                                  },
                                                  () => {
                                                      if (LastSelectedCards == null || LastSelectedCards.Count == 0) {
                                                          return true;
                                                      }

                                                      return !LastSelectedCards.Any(lsc => lsc.Equals(SelectedCard)) &&
                                                             SelectedCard != null && SelectedCard.CardStatus !=
                                                             CardStatusEnum.Removed;
                                                  });


            NewGameCommand = new RelayCommand(async () => {
                                                  if (GameState == GameStateEnum.InProggress) {
                                                      MetroWindow metroWindow = Application
                                                                               .Current.Windows.OfType<GameWindow>()
                                                                               .First();

                                                      MessageDialogResult message =
                                                          await metroWindow.ShowMessageAsync(
                                                              $"Game is in proggress", "Save current game?",
                                                              MessageDialogStyle
                                                                 .AffirmativeAndNegativeAndSingleAuxiliary);
                                                      if (message != MessageDialogResult.Affirmative) {
                                                          return;
                                                      }

                                                      SaveGameCommand.Execute(null);
                                                  }
                                                  //Start game

                                                  ListWidth = Game.Options.Rows * 100;

                                                  Game.StartGame();
                                                  int roundsWon =
                                                      int.Parse(Game.RoundsWon.First(c => char.IsDigit(c)).ToString());
                                                  if (roundsWon == 0) {
                                                      Game.Account.GamesPlayed++;
                                                      Game.Account.Update();
                                                  }

                                                  _paused = false;


                                                  GameState = GameStateEnum.InProggress;
                                              }, () => { return true; });


            SaveGameCommand = new RelayCommand(() => { Game.Save(); }, () => GameState == GameStateEnum.InProggress);

            LoadGameCommand = new RelayCommandObject(param => {
                                                      Debug.WriteLine("Game tryna load");
                                                      Game = GameHelper.Load(
                                                          $"{GameHelper.SavePath(Game)}\\{param as string}");
                                                      ListWidth = Game.Options.Rows * 100;
                                                      SelectedCard = null;
                                                      LastSelectedCards = new List<Card>();
                                                      _paused = false;
                                                  }, param => true);

            LoadGamePathsCommand = new RelayCommand(() => {
                                                        if (!Directory.Exists(GameHelper.SavePath(Game))) {
                                                            Directory.CreateDirectory(GameHelper.SavePath(Game));
                                                        }

                                                        ObservableCollection<SaveGame> savedGames =
                                                            new ObservableCollection<SaveGame>(
                                                                Directory.GetFiles(GameHelper.SavePath(Game), "*.json")
                                                                         .Select(s => new SaveGame(s)));
                                                        if (savedGames.Count == 0) {
                                                            SaveGames = new ObservableCollection<SaveGame>();
                                                        }
                                                        else {
                                                            SaveGames = savedGames;
                                                        }
                                                    }, () => true);

            EditOptionsCommand = new RelayCommand(() => {
                                                      OptionsWindow optionsWindow = new OptionsWindow();
                                                      ((OptionsViewModel) optionsWindow.DataContext).Options =
                                                          Game.Options;
                                                      optionsWindow.Show();
                                                  }, () => !Application.Current.Windows.OfType<Options>().Any());

            StatisticsCommand = new RelayCommand(() => {
                                                     StatisticsWindow _createStatisticsWindow = new StatisticsWindow();
                                                     _createStatisticsWindow.Show();
                                                 });
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        private async void UpdateTime() {
            int CurrentVal() {
                return int.Parse(new string(Game.TimeLeft.Where(char.IsDigit).ToArray()));
            }

            while (true) {
                Thread.Sleep(1000);

                if (_paused) {
                    continue;
                }


                Game.TimeLeft = (CurrentVal() - 1).ToString();


                if (CurrentVal() != 0) {
                    continue;
                }

                _paused = true;
                await Application.Current.Dispatcher.Invoke(async () => {
                                                                MetroWindow metroWindow =
                                                                    Application.Current.Windows.OfType<GameWindow>()
                                                                               .First();
                                                                MessageDialogResult message =
                                                                    await metroWindow.ShowMessageAsync(
                                                                        $"Game Over", "Try again?",
                                                                        MessageDialogStyle.AffirmativeAndNegative);

                                                                if (message == MessageDialogResult.Affirmative) {
                                                                    NewGameCommand.Execute(null);
                                                                }
                                                            });
            }
        }

        public void ResumeGame() {
            _updateTimeTask = Task.Run(() => UpdateTime());
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}