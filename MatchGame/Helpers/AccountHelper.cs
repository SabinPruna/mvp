using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MatchGame.Models;
using Newtonsoft.Json;

namespace MatchGame.Helpers {
    public static class AccountHelper {
        public static void RemoveFiles(this Account account) {
            try {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                File.Delete(GetPathFromUsername(account.Username));
                File.Delete(GetPathForPictures(account.Username));
            }
            catch (Exception ex) {
                Debug.Write(ex.Message);
            }
        }

        public static Account Load(string username) {
            try {
                string jsonString = File.ReadAllText(GetPathFromUsername(username));
                return JsonConvert.DeserializeObject<Account>(jsonString);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static IList<Account> LoadAll() {
            try {
                string[] jsonStrings = Directory.GetFiles(GetPathFromUsername("").Replace(".json", ""), "*.json");

                return jsonStrings.Select(path => JsonConvert.DeserializeObject<Account>(File.ReadAllText(path)))
                                  .ToList();
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static string GetPathFromUsername(string username) {
            return $"{AppContext.BaseDirectory}\\Users\\{username}.json";
        }

        public static string GetPathForPictures(string username) {
            return $"{AppContext.BaseDirectory}\\Users\\Pictures\\{username}.png";
        }
    }
}