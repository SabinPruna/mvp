using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Match.Commands;
using Match.Helpers;
using Match.Models;
using Match.Views;
using Microsoft.Win32;

namespace Match.ViewModels {
    public class AccountViewModel {
        private readonly AccountsViewModel _accountsViewModel;

        #region Properties

        public Account Account { get; set; }

        public ICommand AddAccountCommand { get; set; }
        public ICommand ChooseImageCommand { get; set; }

        public ICommand SaveAccountCommand { get; set; }

        #endregion

        #region Constructors

        public AccountViewModel() { }


        public AccountViewModel(AccountsViewModel accountsViewModel, Account account) {
            SaveAccountCommand = new RelayCommand(SaveAccount);
            ChooseImageCommand = new RelayCommand(ChooseImage);
            AddAccountCommand = new RelayCommand(AddAccount);
            _accountsViewModel = accountsViewModel;
            Account = account;
        }

        #endregion

        private void ChooseImage(object obj) {
            // Configure open file dialog box
            OpenFileDialog dialog = new OpenFileDialog {
                                                           FileName = "Picture",
                                                           DefaultExt = ".png",
                                                           Filter =
                                                               "Image files (*.jpg, *.jpeg, *.gif *.png) | *.jpg; *.jpeg; *.gif *.png"
                                                       };

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result != true) {
                return;
            }

            // Open document
            string filePath = dialog.FileName;
            string pictureName = Guid.NewGuid().ToString();
            string newPath = Path.Combine(PathHelper.ImagesPath, pictureName);
            File.Copy(filePath, newPath);
            Account.ImagePath = newPath;
        }

        private void SaveAccount(object obj) {
            LoginWindow loginScreenWindow = new LoginWindow {DataContext = _accountsViewModel};
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = loginScreenWindow;
            loginScreenWindow.Show();
        }

        private void AddAccount(object obj) {
            _accountsViewModel.Accounts.Add(Account);
            LoginWindow loginScreenWindow = new LoginWindow {DataContext = _accountsViewModel};
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = loginScreenWindow;
            loginScreenWindow.Show();
        }
    }
}