using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatchGame.Helpers;
using MatchGame.Models;

namespace MatchGame.ViewModels {
    public class StatisticsViewModel {
        #region Properties

        public ObservableCollection<Account> Accounts { get; set; }

        #endregion

        #region Constructors

        public StatisticsViewModel() {
            ObservableCollection<Account> accounts = new ObservableCollection<Account>();
            IList<Account> loadedAccounts = AccountHelper.LoadAll();
            foreach (Account account in loadedAccounts) {
                accounts.Add(account);
            }

            Accounts = accounts;
        }

        #endregion
    }
}