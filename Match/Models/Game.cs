using System.Collections.ObjectModel;

namespace Match.Models {
    public class Game {
        #region Properties

        public ObservableCollection<Card> Cards { get; set; }

        public int CurrentRound { get; set; }
        public int Time { get; set; }

        #endregion
    }
}