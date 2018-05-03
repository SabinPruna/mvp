using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MatchGame.Enums;
using MatchGame.Models;
using Newtonsoft.Json;

namespace MatchGame.Helpers {
    public static class GameHelper {
        public static string SavePath(Game game) {
            return $"{AppContext.BaseDirectory}\\Saves\\{game.Account.Username}";
        }

        public static void Save(this Game game) {
            Directory.CreateDirectory(SavePath(game));

            List<int> flippedCardsIndices = game.Cards.Where(c => c.CardStatus == CardStatusEnum.Flipped)
                                                .Select(c => game.Cards.IndexOf(c)).ToList();

            flippedCardsIndices.Select(c => game.Cards[c]).ToList().ForEach(c => c.CardStatus = CardStatusEnum.Normal);
            string path = $"{SavePath(game)}\\{DateTime.Now.ToString().Replace(":", "_").Replace(" ", "_")}.json";
            File.WriteAllText(path, JsonConvert.SerializeObject(game));
            flippedCardsIndices.Select(c => game.Cards[c]).ToList().ForEach(c => c.CardStatus = CardStatusEnum.Flipped);
        }

        public static Game Load(string filePath) {
            return JsonConvert.DeserializeObject<Game>(File.ReadAllText(filePath));
        }
    }
}