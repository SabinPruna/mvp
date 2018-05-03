using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using MatchGame.Helpers;
using Newtonsoft.Json;

namespace MatchGame.Models {
    public class Account : INotifyPropertyChanged {
        private BitmapImage _bitmapImage;
        private string _username;

        #region Properties

        public BitmapImage BitmapImage {
            get => _bitmapImage;
            set {
                if (Equals(value, _bitmapImage)) {
                    return;
                }

                _bitmapImage = value;

                OnPropertyChanged();
            }
        }

        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }

        public List<int> HighScores { get; set; }

        public string Username {
            get => _username;
            set {
                if (value == _username) {
                    return;
                }

                _username = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Used for creating a new account
        /// </summary>
        /// <param name="username">The username.</param>
        public Account(string username) {
            Username = username;
            HighScores = new List<int>();
            GamesPlayed = 0;
            GamesWon = 0;
        }

        [JsonConstructor]
        public Account(string username, string bitmapImage, List<int> highScores, int gamesPlayed, int gamesWon) :
            this(username) {

            if (bitmapImage == null) {
                BitmapImage = null;
            }
            else {
                BitmapImage = new BitmapImage();
                BitmapImage.BeginInit();
                BitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                BitmapImage.UriSource = new Uri(bitmapImage);
                BitmapImage.EndInit();
            }

            GamesPlayed = gamesPlayed;
            GamesWon = gamesWon;
            HighScores = highScores;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void Save() {
            try {
                if (!BitmapImage.UriSource.AbsolutePath.Contains("Users\\Pictures") && BitmapImage != null) {
                    if (File.Exists(AccountHelper.GetPathForPictures(Username))) {
                        File.Delete(AccountHelper.GetPathForPictures(Username));
                    }

                    File.Copy(BitmapImage.UriSource.AbsolutePath, AccountHelper.GetPathForPictures(Username));
                    BitmapImage = new BitmapImage(new Uri(AccountHelper.GetPathForPictures(Username)));
                }

                string jsonString = JsonConvert.SerializeObject(this);
                File.WriteAllText(AccountHelper.GetPathFromUsername(Username), jsonString);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Update() {
            try {
                Application.Current.Dispatcher.Invoke(() => {
                                                          string jsonString = JsonConvert.SerializeObject(this);
                                                          File.WriteAllText(
                                                              AccountHelper.GetPathFromUsername(Username), jsonString);
                                                      });
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj) {
            return obj is Account account &&
                   Username == account.Username;
        }

        public override int GetHashCode() {
            int hashCode = 2138401232;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_username);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Username);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(HighScores);
            return hashCode;
        }

        public override string ToString() {
            return $"{Username}";
        }
    }
}