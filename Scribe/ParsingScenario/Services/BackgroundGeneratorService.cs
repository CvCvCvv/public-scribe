using Application.Abstractions.Consts;
using Application.Abstractions.Domains;
using ImageGenerators.Generators;
using ImageGenerators.Interfaces;

namespace ParsingScenario.Services
{
    public class BackgroundGeneratorService
    {
        private readonly IGeneratorImage _generator;

        public BackgroundGeneratorService(string secret, string key, string url)
        {
            switch (Settings.BackgroundGenerator) 
            {
                case ImageGeneratorProvider.Kadinsky:
                    _generator = new KadinskyGenerator(secret, key);
                    break;
                case ImageGeneratorProvider.StableDiffusion:
                    _generator = new StableDiffusionGenerator(url);
                    break;
                default:
                    _generator = new KadinskyGenerator(secret, key);
                    break;
            }
        }

        public Task GenImages(List<ImagePath> imagePaths)
        {
            Directory.CreateDirectory(RenPyCodeHelper.BackgroundImagesFolderPath);

            List<Thread> threadsGenerator = new List<Thread>();

            foreach (ImagePath imagePath in imagePaths)
            {
                Thread generator = new Thread(a => { SaveFile(imagePath, RenPyCodeHelper.BackgroundImagesFolderPath).Wait(); });
                threadsGenerator.Add(generator);
                generator.Start();
            }
            foreach (Thread generator in threadsGenerator)
            {
                generator.Join();
            }

            return Task.CompletedTask;
        }

        private async Task SaveFile(ImagePath imagePath, string dirPath)
        {
            var (photo, result) = await _generator.GenerateImageToBase64(imagePath.PromtImage, 1920, 1080);

            if (!result)
            { 
                photo = Settings.TemplateImage;
                await Console.Out.WriteLineAsync($"Error Generate image: {imagePath.PathToFile}\nSending template...");
            }
            else
            {
                await Console.Out.WriteLineAsync($"Generate image: {imagePath.PathToFile}");
            }

            byte[] data = Convert.FromBase64String(photo!);
            File.WriteAllBytes(Path.Combine(dirPath, imagePath.PathToFile), data);
        }
    }
}
