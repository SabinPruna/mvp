using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Match.Commands;
using Match.Helpers;
using Match.Models;
using Match.Views;
using Newtonsoft.Json;

namespace Match.ViewModels {
    public class AccountsViewModel {
        #region Properties

        public ObservableCollection<Account> Accounts { get; set; }

        public ICommand DeleteAccountCommand { get; set; }

        public ICommand EditAccountCommand { get; set; }

        public ICommand NewAccountCommand { get; set; }

        public ICommand PlayGameCommand { get; set; }

        public Account SelectedAccount { get; set; }

        #endregion

        #region Constructors

        public AccountsViewModel() {
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
            EditAccountCommand = new RelayCommand(EditAccount);
            NewAccountCommand = new RelayCommand(NewAccount);
            PlayGameCommand = new RelayCommand(PlayGame);

            LoadAccounts();
        }

        #endregion

        private void PlayGame(object obj) {
            GameWindow gameWindow = new GameWindow { DataContext = new GameViewModel(this) };
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = gameWindow;
            gameWindow.Show();
        }

        private void NewAccount(object obj) {
            AddAccountWindow newAccountWindow =
                new AddAccountWindow {DataContext = new AccountViewModel(this, new Account())};
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = newAccountWindow;
            newAccountWindow.Show();
        }

        private void EditAccount(object obj) {
            EditAccountWindow editAccountWindow =
                new EditAccountWindow {DataContext = new AccountViewModel(this, SelectedAccount)};
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = editAccountWindow;
            editAccountWindow.Show();
        }

        private void DeleteAccount(object obj) {
            Accounts.Remove(SelectedAccount);
        }

        public void LoadAccounts() {
            string fileContents = File.ReadAllText(PathHelper.AccountsJsonPath);
            Accounts = JsonConvert.DeserializeObject<ObservableCollection<Account>>(fileContents) ??
                       new ObservableCollection<Account>();
        }

        public void SaveAccounts() {
            File.WriteAllText(PathHelper.AccountsJsonPath, JsonConvert.SerializeObject(Accounts));
        }

        ~AccountsViewModel() {
            SaveAccounts();
        }
    }
}