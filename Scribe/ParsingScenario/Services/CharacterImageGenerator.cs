using Application.Abstractions.Consts;
using Application.Abstractions.Domains;
using ImageGenerators.Generators;
using ImageGenerators.Helpers;
using ImageGenerators.Interfaces;

namespace ParsingScenario.Services
{
    public class CharacterImageGenerator
    {
        private readonly IGeneratorImage _generator;
        private readonly bool _useImg2Img = false;
        private readonly RemoveBackgroundService _removeBackgroundService;

        public CharacterImageGenerator(string key, string secret, string url)
        {
            _removeBackgroundService = new RemoveBackgroundService();

            _useImg2Img = Settings.UseImg2Img;

            switch (Settings.BackgroundGenerator)
            {
                case ImageGeneratorProvider.Kadinsky:
                    _useImg2Img = false;
                    _generator = new KadinskyGenerator(secret, key);
                    break;
                case ImageGeneratorProvider.StableDiffusion:
                    _generator = new StableDiffusionGenerator(url);
                    break;
                default:
                    _useImg2Img = false;
                    _generator = new KadinskyGenerator(secret, key);
                    break;
            }
        }

        public Task GenerateAllCharacter(List<RenPyCommand> commands, List<CharacterDescription> characters)
        {
           if(_useImg2Img)
                GenerateFromImg2Img(commands, characters);
           else
                GenerateFromText2Img(commands, characters);

            return Task.CompletedTask;
        }

        public Task GenerateAllCharacter(List<ImagePath> images)
        {
            if (_useImg2Img)
                Console.WriteLine("Не используем img2img так как отсутсвуют базовые описания персонажей");

            GenerateFromText2Img(images);

            return Task.CompletedTask;
        }

        private Task GenerateFromImg2Img(List<RenPyCommand> commands, List<CharacterDescription> characters)
        {
            List<Thread> threadsGenerator = new List<Thread>();
            List<Thread> threadsGeneratortempalte = new List<Thread>();

            foreach (var character in characters)
            {
                var filepath = CreateFile(character.Name, true);
                threadsGeneratortempalte.Add(new Thread(a => { Generator(character.Description + ", " + KadinskyPromtHelper.CharacterGenerate, character.Name, filepath).Wait(); }));
            }

            ThreadsRunner(threadsGeneratortempalte);

            foreach (var command in commands)
            {
                if (command.Gesture is not null)
                {
                    var description = characters.Where(a => a.Name == command.Type).FirstOrDefault()!.Description;
                    var filepath = CreateFile(command.Type);

                    Thread generator = new Thread(a => { Generator(command.Gesture, command.Type, filepath, true).Wait(); });
                    threadsGenerator.Add(generator);
                }
            }

            ThreadsRunner(threadsGenerator);

            return Task.CompletedTask;
        }

        private void ThreadsRunner(List<Thread> threads)
        {
            foreach (Thread thread in threads)
            {
                thread.Start();
                Thread.Sleep(1000);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        private Task GenerateFromText2Img(List<ImagePath> images)
        {
            List<Thread> threadsGenerator = new List<Thread>();

            foreach (var command in images)
            {

                   
                    var filepath = CreateFileByName(command.ImageName);

                    Thread generator = new Thread(a => { Generator(command.PromtImage + ", " + KadinskyPromtHelper.CharacterGenerate, command.ImageName, filepath).Wait(); });
                    threadsGenerator.Add(generator);
            }

            foreach (Thread generator in threadsGenerator)
            {
                generator.Start();
                Thread.Sleep(1000);
            }

            foreach (Thread generator in threadsGenerator)
            {
                generator.Join();
            }

            return Task.CompletedTask;
        }

        private Task GenerateFromText2Img(List<RenPyCommand> commands, List<CharacterDescription> characters)
        {
            List<Thread> threadsGenerator = new List<Thread>();

            foreach (var command in commands)
            {
                if (command.Gesture is not null)
                {
                    var description = characters.Where(a => a.Name == command.Type).FirstOrDefault()!.Description;
                    var filepath = CreateFile(command.Type);

                    Thread generator = new Thread(a => { Generator(description + ", " + command.Gesture + ", " + KadinskyPromtHelper.CharacterGenerate, command.Type, filepath).Wait(); });
                    threadsGenerator.Add(generator);
                }
            }

            foreach (Thread generator in threadsGenerator)
            {
                generator.Start();
                Thread.Sleep(1000);
            }

            foreach (Thread generator in threadsGenerator)
            {
                generator.Join();
            }

            return Task.CompletedTask;
        }

        private async Task Generator(string prompt, string characterName, string filepath, bool useImg2Img = false)
        {
            string? photo;
            bool isSuccess;

            var path = Path.Combine(RenPyCodeHelper.CharacterImageFolderPathAbsolute, characterName, characterName + ".jpg");

            if (useImg2Img)
                (photo, isSuccess) = await _generator.GenerateImage2ImageToBase64(prompt, Convert.ToBase64String(File.ReadAllBytes(path)), 1080, 1080);
            else
                (photo, isSuccess) = await _generator.GenerateImageToBase64(prompt, 1080, 1080);

            if (!isSuccess)
            {
                //TODO какой нибудь шаблон
                photo = Settings.TemplateImage;
            }
            else
            {
                bool removing;
                (photo, removing) = await _removeBackgroundService.RemoveBackground(photo!);
                if (!removing)
                    photo = Settings.TemplateImage;
            }



            byte[] data = Convert.FromBase64String(photo!);
            File.WriteAllBytes(filepath, data);

            await Console.Out.WriteLineAsync($"Generate image character {characterName}: {filepath}");            
        }


        private string CreateFile(string name, bool template = false)
        {
            var path = Path.Combine(RenPyCodeHelper.CharacterImageFolderPathAbsolute, name);
            Directory.CreateDirectory(path);

            var filename = "";
            if (template)
                filename = name + ".jpg";
            else
            {
                if (_useImg2Img)
                    filename = (Directory.GetFiles(path).Length).ToString() + ".png";
                else
                    filename = (Directory.GetFiles(path).Length + 1).ToString() + ".png";
            }

            var filepath = Path.Combine(path, filename);

            using var a = File.Create(filepath);

            return filepath;
        }

        private string CreateFileByName(string name)
        {
            var path = Path.Combine(RenPyCodeHelper.CharacterImageFolderPathAbsolute);
            Directory.CreateDirectory(path);

            var filename = name + ".png";
            

            var filepath = Path.Combine(path, filename);

            using var a = File.Create(filepath);

            return filepath;
        }
    }
}
