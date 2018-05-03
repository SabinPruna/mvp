using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Match.Annotations;
using Match.Helpers;

namespace Match.Models
{
    public class Card : INotifyPropertyChanged
    {
        private string _imagePath;
        private bool _isShown;
        private Card _pair;
        private bool _foundPair;

        public string ImagePath
        {
            get
            {
                if (FoundPair)
                {
                    return null;
                }
                return IsShown ? _imagePath : PathHelper.BackImage;
            }
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public bool IsShown
        {
            get { return _isShown; }
            set
            {
                _isShown = value;
                OnPropertyChanged(nameof(IsShown));
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public Card Pair
        {
            get { return _pair; }
            set
            {
                _pair = value;
                OnPropertyChanged(nameof(Pair));

            }
        }

        public bool FoundPair
        {
            get { return _foundPair; }
            set
            {
                _foundPair = value;
                OnPropertyChanged(nameof(FoundPair));
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return _imagePath;
        }
    }
}
