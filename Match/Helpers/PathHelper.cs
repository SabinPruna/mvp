using System.IO;

namespace Match.Helpers {
    public class PathHelper {
        public static readonly string StoragePath;
        public static readonly string ImagesPath;
        public static readonly string CurrentDirectoryPath;
        public static readonly string AccountsJsonPath;
        public static readonly string AccountsJsonName;
        public static readonly string CardImagesPath;

        public static readonly string BackImage;

        #region Constructors

        static PathHelper() {
            CurrentDirectoryPath = Directory.GetCurrentDirectory();
            StoragePath = Path.Combine(CurrentDirectoryPath, @"Resources\Storage");
            ImagesPath = Path.Combine(CurrentDirectoryPath, @"Resources\Images");
            AccountsJsonName = "Accounts.json";
            AccountsJsonPath = Path.Combine(StoragePath, AccountsJsonName);
            CardImagesPath = Path.Combine(CurrentDirectoryPath, @"Resources\CardImages");
            BackImage = Path.Combine(CurrentDirectoryPath, @"Resources\CardBackground.jpg");
        }

        #endregion
    }
}