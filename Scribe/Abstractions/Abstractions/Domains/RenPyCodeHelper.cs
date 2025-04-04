namespace Application.Abstractions.Domains
{
    public static class RenPyCodeHelper
    {
        public const string InitBlock = "init:";

        // Названия файлов и директорий
        public static string RootStoryPath = "";
        private static string _generalDirectory
        {
            get
            {
                return Path.Combine("scenario", RootStoryPath);
            }
        }

        private const string _renPyScriptPath = "script.rpy";
        private const string _metadataFile = "meta.data";
        private const string _backgroundImagesPath = "backgrounds";
        //
        public static string BackgroundImagesFolderPath => Path.Combine(_generalDirectory, _backgroundImagesPath);
        public static string CharacterImageFolderPathAbsolute => Path.Combine(_generalDirectory, CharacterImageFolderPath);
        public static string CharacterVoiceFolderPathAbsolute => Path.Combine(_generalDirectory, CharacterVoiceFolderPath);

        public const string CharacterImageFolderPath = "characters";
        public const string CharacterVoiceFolderPath = "voices";
        //
        public static string RenPyScriptPath => Path.Combine(_generalDirectory, _renPyScriptPath);
        public static string MetadataFilePath => Path.Combine(_generalDirectory, _metadataFile);
        //

        public static string GetBackgroundsPath()
        {
            return _backgroundImagesPath;
        }

        public static string GetDirectory()
        {
            return _generalDirectory;
        }

        public static string GetCharacterInit(string characterName)
        {
            return $"    $ {characterName.Replace(" ", "")} = Character('{characterName}', color=(200, 255, 200, 255))";
        }

        public static string GetCharacterPhrase(string characterName, string phrase)
        {
            return $"    {characterName.Replace(" ", "")} \"{phrase}\"";
        }

        public static string GetCharacterPicture(string characterName, string characterEmotion, string file)
        {
            return $"    image {characterName.Replace(" ", "")} {characterEmotion}_em = \"{CharacterImageFolderPath}/{characterName}/{file}\"";
        }

        public static string GetBackgroundPucture(string image, string file)
        {
            return $"    image {image} = \"{_backgroundImagesPath}/{file}\"";
        }

        public static string GetShowBackground(string background, string param = "")
        {
            return $"    scene {background} {param}";
        }

        public static string GetShowCharacterEmotion(string character, string characterEmotion, string param = "")
        {
            return $"    show {character.Replace(" ", "")} {characterEmotion}_em {param}";
        }

        public static string GetCahracterVoice(string character, string voiceFile)
        {
            return $"    voice \"{CharacterVoiceFolderPath}/{character.Replace(" ", "")}/{voiceFile}.wav\"";
        }
    }
}
