using Application.Abstractions.Domains;

namespace ParsingScenario.Services
{
    public class RenPyRunnerService
    {
        private const string _pathRenPyGame = "path";
        public void Run()
        {
            if (Directory.Exists(Path.Combine(_pathRenPyGame, RenPyCodeHelper.CharacterImageFolderPath)))
                Directory.Delete(Path.Combine(_pathRenPyGame, RenPyCodeHelper.CharacterImageFolderPath), true);

            if (Directory.Exists(Path.Combine(_pathRenPyGame, RenPyCodeHelper.GetBackgroundsPath())))
                Directory.Delete(Path.Combine(_pathRenPyGame, RenPyCodeHelper.GetBackgroundsPath()), true);

            if (Directory.Exists(Path.Combine(_pathRenPyGame, RenPyCodeHelper.CharacterVoiceFolderPath)))
                Directory.Delete(Path.Combine(_pathRenPyGame, RenPyCodeHelper.CharacterVoiceFolderPath), true);

            if (File.Exists(Path.Combine(_pathRenPyGame, "script.rpy")))
                File.Delete(Path.Combine(_pathRenPyGame, "script.rpy"));

            Directory.Move(RenPyCodeHelper.CharacterImageFolderPathAbsolute, Path.Combine(_pathRenPyGame, RenPyCodeHelper.CharacterImageFolderPath));
            Directory.Move(RenPyCodeHelper.BackgroundImagesFolderPath, Path.Combine(_pathRenPyGame, RenPyCodeHelper.GetBackgroundsPath()));
            Directory.Move(RenPyCodeHelper.CharacterVoiceFolderPathAbsolute, Path.Combine(_pathRenPyGame, RenPyCodeHelper.CharacterVoiceFolderPath));

            File.Move(RenPyCodeHelper.RenPyScriptPath, Path.Combine( _pathRenPyGame, "script.rpy"));
        }

        public static void Cleaner()
        {
            if(Directory.Exists(RenPyCodeHelper.GetDirectory()))
                Directory.Delete(RenPyCodeHelper.GetDirectory(), true);
            else
                Directory.CreateDirectory(RenPyCodeHelper.GetDirectory());
        }
    }
}
