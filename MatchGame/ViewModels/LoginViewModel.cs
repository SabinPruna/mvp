using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MatchGame.Commands;
using MatchGame.Helpers;
using MatchGame.Models;
using MatchGame.Views;

namespace MatchGame.ViewModels {
    public class LoginViewModel {
        #region Properties

        public ObservableCollection<Account> Accounts { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        public Account SelectedAccount { get; set; }

        #endregion

        #region Constructors

        public LoginViewModel() {
            Accounts = new ObservableCollection<Account>(AccountHelper.LoadAll());
            SelectedAccount = Accounts.FirstOrDefault();

            LoginCommand = new RelayCommand(() => {
                                                Application.Current.Windows.OfType<LoginWindow>().First().Hide();

                                                GameWindow gameWindow = new GameWindow();
                                                ((GameViewModel) gameWindow.DataContext).Game.Account = SelectedAccount;
                                                gameWindow.Show();

                                                Debug.WriteLine($"{SelectedAccount.Username} has just logged in");
                                            },
                                            () => {
                                                if (SelectedAccount == null) {
                                                    return false;
                                                }

                                                Account localAccount = AccountHelper.Load(SelectedAccount.Username);
                                                return SelectedAccount.Equals(localAccount);
                                            });

            RegisterCommand = new RelayCommand(() => {
                                                   RegisterWindow registerWindow = new RegisterWindow();
                                                   Application.Current.MainWindow = registerWindow;
                                                   registerWindow.DataContext = new RegisterViewModel(this);
                                                   registerWindow.Show();
                                                   Debug.WriteLine($"Register stared");
                                               }, () => true);

            DeleteCommand = new RelayCommand(async () => {
                                                 MetroWindow metroWindow = Application
                                                                          .Current.Windows.OfType<LoginWindow>()
                                                                          .First();

                                                 MessageDialogResult message =
                                                     await metroWindow.ShowMessageAsync(
                                                         $"Deleting {SelectedAccount.Username}", "Are you sure?",
                                                         MessageDialogStyle.AffirmativeAndNegative);
                                                 if (message == MessageDialogResult.Affirmative) {
                                                     SelectedAccount.RemoveFiles();
                                                     Accounts.Remove(SelectedAccount);
                                                     SelectedAccount = null;
                                                 }
                                             },
                                             () => SelectedAccount != null);
        }

        #endregion
    }
}