
using System.Diagnostics;
using Streamer.Application.Abstractions.Models.GenerateStory;
namespace Streamer.BackgroundServices.RenPyRunner
{
    public static class RenPyRunner
    {
        public static string PathScenarios = "";
        public static string PathRenPyGame = "";
        public static string NameExeFile = "";
        public static string PathExeFile = "";
        public static int Timeout;

        public static Task Run(StreamerStoryInfoModel infoStory)
        {

            var sourcePath = Path.Combine(PathScenarios, infoStory.Id.ToString());

            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine($"История с GIUD {infoStory.Id} не найдена, пропускаю...");

                return Task.CompletedTask;
            }

            Console.WriteLine($"Начали спектакль. Режиссёр-постановщик - Mistral, автор сценария - {infoStory.Story.Author}, GUID - {infoStory.Id}");

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var pathFile = newPath.Replace(sourcePath, PathRenPyGame);

                var e = Path.GetDirectoryName(pathFile)!;

                if (!Directory.Exists(Path.GetDirectoryName(pathFile)!))
                    Directory.CreateDirectory(Path.GetDirectoryName(pathFile)!);
                
                File.Copy(newPath, pathFile, true);
            }


            var startInfo = new ProcessStartInfo() { 
                WorkingDirectory = PathExeFile,
                FileName = Path.Combine(PathExeFile, NameExeFile)
            };

            Thread.Sleep(15000);

            var a = Process.Start(startInfo)!;

            if (a.WaitForExit(TimeSpan.FromMinutes(5)))
            {
                a.CloseMainWindow();
                a.Close();
            }

            Console.WriteLine("Закончили спектакль");

            return Task.CompletedTask;
            //a.Close();
        }
    }
}
