using Application.Abstractions.Consts;
using Application.Abstractions.Domains;
using Application.Abstractions.Domains.Models.GenerateStory;
using LLMWorkers.Interfaces;
using LLMWorkers.Workers;
using ParsingScenario.Services;
using System.Text.Json;

namespace Scribe.Service
{
    internal class ShapeStory
    {
        private static string idProject = "";
        private static string locked = "_locked";

        public static async Task Shape(GenerateStoryModel theme)
        {
            ILLMWorker worker;

            switch (Settings.LLMConnecting)
            {
                case LLMProvider.ChatGPT:
                    worker = new ChatGPTWorker("token");
                    break;
                case LLMProvider.Mistral:
                    worker = new MistralWorker("token");
                    break;
                default:
                    worker = new ChatGPTWorker("token");
                    break;
            }

            string result = await worker.GetScenario(theme.Theme);

            Console.WriteLine(result);

            idProject = Guid.NewGuid().ToString();
            RenPyCodeHelper.RootStoryPath = idProject + locked;
            SaveMetadata(theme);

            ParsingScenario.ParsingScenario parsing = new(result + "\n");

            Thread parser = new(async a => await parsing.Parse());

            CharacterImageGenerator characterGenerator = new("token", "token", "uri");
            BackgroundGeneratorService backgroundGenerator = new("token", "token", "uri");
            VoiceGeneratorService voiceGenerator = new("http://localhost:8085");

            Thread characterEmotionGen = new(async a => await characterGenerator.GenerateAllCharacter(parsing.CharacterImagesPath));
            Thread backgroundGenerate = new(a => backgroundGenerator.GenImages(parsing.ImagesPath).Wait());
            Thread voiceover = new(a => voiceGenerator.Generating(parsing.CharacterPhrases).Wait());

            Thread.Sleep(1000);
            backgroundGenerate.Start();
            characterEmotionGen.Start();

            voiceover.Start();

            characterEmotionGen.Join();
            voiceover.Join();
            backgroundGenerate.Join();

            parser.Start();
            parser.Join();

            var oldFolder = RenPyCodeHelper.GetDirectory();
            RenPyCodeHelper.RootStoryPath = idProject;
            Directory.Move(oldFolder, RenPyCodeHelper.GetDirectory());
        }

        private static void SaveMetadata(GenerateStoryModel model)
        {
            if(!Directory.Exists(RenPyCodeHelper.GetDirectory()))
                Directory.CreateDirectory(RenPyCodeHelper.GetDirectory());

            File.WriteAllText(RenPyCodeHelper.MetadataFilePath, JsonSerializer.Serialize(model));
        }
    }
}
