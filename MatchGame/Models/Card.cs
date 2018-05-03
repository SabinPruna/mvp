using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using MatchGame.Enums;
using MatchGame.Helpers;
using Newtonsoft.Json;

namespace MatchGame.Models {
    public class Card : INotifyPropertyChanged {
        [JsonProperty] private BitmapImage _bitmapImage;

        private CardStatusEnum _cardStatus;

        #region Properties

        [JsonIgnore]
        public BitmapImage BitmapImage {
            get {
                switch (CardStatus) {
                    case CardStatusEnum.Normal:
                        return CardUtils.DefaultCardCoverImage;
                    case CardStatusEnum.Flipped:
                        return _bitmapImage;
                    case CardStatusEnum.Removed:
                        return null;
                    case CardStatusEnum.Selected:
                        return CardUtils.SelectedCardCoverImage;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                if (Equals(value, _bitmapImage)) {
                    return;
                }

                _bitmapImage = value;
                OnPropertyChanged();
            }
        }

        public CardStatusEnum CardStatus {
            get => _cardStatus;
            set {
                if (value == _cardStatus) {
                    return;
                }

                _cardStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BitmapImage));
            }
        }

        #endregion

        #region Constructors

        public Card(BitmapImage bitmapImage) {
            BitmapImage = bitmapImage;
            CardStatus = CardStatusEnum.Flipped;
        }

        [JsonConstructor]
        public Card(string bitmapImage, CardStatusEnum cardStatus) {
            CardStatus = cardStatus;

            BitmapImage = bitmapImage == null ? null : new BitmapImage(new Uri(bitmapImage));
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}