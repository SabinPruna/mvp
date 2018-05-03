using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MatchGame.Commands;
using MatchGame.Models;
using Microsoft.Win32;

namespace MatchGame.ViewModels {
    public class RegisterViewModel {
        #region Properties

        public Account Account { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SelectProfileImageCommand { get; set; }

        #endregion

        #region Constructors

        public RegisterViewModel() {
            Account = new Account("");
        }

        public RegisterViewModel(LoginViewModel loginViewModel) {
            Account = new Account("");
            SaveCommand = new RelayCommand(() => {
                                               Account.Save();
                                               loginViewModel.Accounts.Add(Account);
                                               MetroWindow metroWindow = Application.Current.MainWindow as MetroWindow;
                                               metroWindow?.Close();
                                               IEnumerator iEnumerator = Application.Current.Windows.GetEnumerator();
                                               iEnumerator.MoveNext();
                                               Application.Current.MainWindow = iEnumerator.Current as MetroWindow;
                                               Debug.WriteLine("Save button pressed");
                                           }, () => { return !string.IsNullOrEmpty(Account.Username); });

            SelectProfileImageCommand = new RelayCommand(() => {
                                                             FileDialog fileDialog = new OpenFileDialog();
                                                             fileDialog.ShowDialog();
                                                             if (fileDialog.CheckFileExists &&
                                                                 fileDialog.FileName != string.Empty) {
                                                                 Account.BitmapImage =
                                                                     new BitmapImage(new Uri(fileDialog.FileName));
                                                             }
                                                         }, () => true);
        }

        #endregion
    }
}