using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using mvp;
using Match.Commands;
using Match.Helpers;
using Match.Models;
using Match.Views;
using Newtonsoft.Json;

namespace Match.ViewModels {
    public class GameViewModel : INotifyPropertyChanged {
        private readonly AccountsViewModel _accountsViewModel;
        private readonly List<string> _imagePaths;
        private int _currentRound;

        private DispatcherTimer _dispatcherTimer;

        private int _time;

        #region Properties

        public ICommand AboutCommand { get; set; }

        public Account Account { get; set; }

        public ObservableCollection<Card> Cards { get; set; }

        public ICommand CardSelectedCommand { get; set; }

        public int CurrentRound {
            get => _currentRound;
            set {
                _currentRound = _currentRound == 3 ? 0 : value;

                OnPropertyChanged(nameof(RoundsRemaining));
            }
        }

        public ICommand ExitCommand { get; set; }

        public ICommand LoadGameCommand { get; set; }

        public ICommand NewGameCommand { get; set; }

        public string RoundsRemaining => $"{CurrentRound}/3";

        public ICommand SaveGameCommand { get; set; }

        public Card SelectedCard { get; set; }
        public ICommand StatisticsCommand { get; set; }

        public int Time {
            get => _time;
            set {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        #endregion

        #region Constructors

        public GameViewModel(AccountsViewModel accountsViewModel) : this() {
            NewGameCommand = new RelayCommand(NewGame);
            SaveGameCommand = new RelayCommand(SaveGame);
            LoadGameCommand = new RelayCommand(LoadGame);
            AboutCommand = new RelayCommand(OpenAbout);
            StatisticsCommand = new RelayCommand(Statistics);
            ExitCommand = new RelayCommand(Exit);
            CardSelectedCommand = new RelayCommand(CardSelected);

            _accountsViewModel = accountsViewModel;
            Account = accountsViewModel.SelectedAccount;
            Account.AccountSettings.GridSizeX = 4;
            Account.AccountSettings.GridSizeY = 4;
        }

        public GameViewModel() {
            _imagePaths = new List<string>();
            _imagePaths = Directory.GetFiles(PathHelper.CardImagesPath, "*.jpg", SearchOption.AllDirectories).ToList();
            Cards = new ObservableCollection<Card>();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void Exit(object obj) {
            Application.Current.Shutdown();
        }

        private void Statistics(object obj) {
            StatisticsWindow statisticsWindow = new StatisticsWindow {DataContext = _accountsViewModel};
            statisticsWindow.Show();
        }

        private void OpenAbout(object obj) {
            AboutWindow window = new AboutWindow();
            window.Show();
        }

        private void LoadGame(object obj) {
            string file = Path.Combine(PathHelper.StoragePath, Account.Username);
            if (File.Exists(file)) {
                Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(file));

                CurrentRound = game.CurrentRound;
                Time = game.Time;
                Cards.Clear();

                foreach (Card gameCard in game.Cards) {
                    Cards.Add(gameCard);
                }

                ReestablishPairs();
            }
            else {
                MessageBox.Show("There is no savefile for this Account");
            }
        }

        private void ReestablishPairs() {
            foreach (Card card in Cards) {
                card.IsShown = true;
            }


            foreach (Card card in Cards) {
                card.Pair = Cards.ToList().Find(c => c.ImagePath == card.ImagePath && !card.Equals(c));
            }

            foreach (Card card in Cards) {
                card.IsShown = false;
            }
        }

        private void SaveGame(object obj) {
            string file = Path.Combine(PathHelper.StoragePath, Account.Username);
            if (!File.Exists(file)) {
                using (File.Create(file)) { }
            }

            foreach (Card card in Cards) {
                card.IsShown = true;
            }

            Game game = new Game {
                                     Cards = Cards,
                                     CurrentRound = CurrentRound,
                                     Time = Time
                                 };

            File.WriteAllText(file, JsonConvert.SerializeObject(game, Formatting.None,
                                                                new JsonSerializerSettings {
                                                                                               ReferenceLoopHandling =
                                                                                                   ReferenceLoopHandling
                                                                                                      .Ignore
                                                                                           }));

            foreach (Card card in Cards) {
                card.IsShown = false;
            }
        }

        private void NewGame(object obj) {
            CurrentRound = 1;
            LoadCardGrid();
            StartTimer();
        }

        private void StartTimer() {
            Time = Account.AccountSettings.Time != 0 ? Account.AccountSettings.Time : 60;
            _dispatcherTimer?.Stop();
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += (o, args) => {
                                         Time--;
                                         if (Time <= 0) {
                                             _dispatcherTimer.Stop();
                                             Account.GamesLost++;
                                             MessageBox.Show("Time ran out!");
                                         }
                                     };
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();
        }

        private async void CardSelected(object obj) {
            if (Time < 1) {
                return;
            }

            if (Cards.Count(t => t.IsShown) > 1) {
                return;
            }

            Card selectedCard = SelectedCard;
            if (selectedCard == null) {
                return;
            }

            if (selectedCard.FoundPair) {
                return;
            }

            selectedCard.IsShown = true;
            if (Cards.Count(t => t.IsShown) < 2) {
                return;
            }

            await Task.Delay(500);

            if (selectedCard.Pair.IsShown) {
                selectedCard.Pair.FoundPair = true;
                selectedCard.FoundPair = true;
            }

            foreach (Card card in Cards) {
                card.IsShown = false;
            }

            if (Cards.Count(t => !t.FoundPair) <= 0) {
                if (CurrentRound++ == 3) {
                    MessageBox.Show("You won!");
                    _dispatcherTimer.Stop();
                    Account.GamesWon++;
                    return;
                }

                LoadCardGrid();
                StartTimer();
            }
        }

        private void LoadCardGrid() {
            int n = Account.AccountSettings.GridSizeX * Account.AccountSettings.GridSizeY / 2;
            Cards.Clear();
            Random r = new Random();
            for (int k = 0; k < n; k++) {
                int index = r.Next(_imagePaths.Count);
                while (Cards.Any(t => t.ImagePath == _imagePaths[index])) {
                    index = r.Next(_imagePaths.Count);
                }

                Card card1 = new Card {ImagePath = _imagePaths[index], IsShown = true};
                Card card2 = new Card {ImagePath = _imagePaths[index], IsShown = true};
                card1.Pair = card2;
                card2.Pair = card1;

                Cards.Add(card1);
                Cards.Add(card2);
            }

            Cards.Shuffle();
            foreach (Card Card in Cards) {
                Card.IsShown = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}