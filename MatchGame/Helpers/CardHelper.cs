using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using MatchGame.Models;

namespace MatchGame.Helpers {
    public static class CardUtils {
        private static readonly Random Random;

        private static readonly Random Rng = new Random();

        #region Properties

        public static List<BitmapImage> CardSet { get; set; }
        public static BitmapImage DefaultCardCoverImage { get; set; }
        public static BitmapImage SelectedCardCoverImage { get; set; }

        #endregion

        #region Constructors

        static CardUtils() {
            DefaultCardCoverImage = new BitmapImage(new Uri($"{AppContext.BaseDirectory}\\Cards\\DefaultCover.png"));
            SelectedCardCoverImage = new BitmapImage(new Uri($"{AppContext.BaseDirectory}\\Cards\\SelectedCover.png"));
            List<string> cardPaths = Directory.GetFiles($"{AppContext.BaseDirectory}\\Cards\\Set1\\", "*.png").ToList();
            CardSet = cardPaths.Select(card => new BitmapImage(new Uri(card))).ToList();
            Random = new Random();
        }

        #endregion

        private static BitmapImage GetCompletlyRandomCard() {
            return CardSet[Random.Next(0, CardSet.Count)];
        }

        public static BitmapImage GetRandomCard(this IList<Card> cards) {
            BitmapImage bitmapImage = GetCompletlyRandomCard();
            Application.Current.Dispatcher.Invoke(async () => {
                                                      while (cards.Any(
                                                          card =>
                                                              card.BitmapImage.UriSource.Equals(bitmapImage.UriSource))
                                                      ) {
                                                          bitmapImage = GetCompletlyRandomCard();
                                                      }
                                                  });

            return bitmapImage;
        }

        public static void Shuffle<T>(this IList<T> list) {
            int count = list.Count;
            while (count > 1) {
                count--;
                int randomElementKey = Rng.Next(count + 1);
                T value = list[randomElementKey];
                list[randomElementKey] = list[count];
                list[count] = value;
            }
        }
    }
}