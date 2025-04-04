using Application.Abstractions.Domains;
using ParsingScenario.Abstractions;
using System.Text;

namespace ParsingScenario
{
    public class ParsingScenarioDeprecated : IParsingScenario
    {
        // Маячки для парсера
        private const string _dalleLink = "{DALL-E}: \"";
        private const string _characterInitial = "+";
        private const string _characterFinal = " - \"";
        private const string _characterPhrase = "[";
        private const string _characterPhraseEndInit = "]: \"";
        //

        // Лексемы для RenPy скрипта
        private const string _dalleNameInScenario = "image";
        //

        private string[] _positions = { " at left", " at center", " at right" };
        private string[] _moves = { " with moveinleft", " with moveinbottom", " with moveinright" };

        public List<CharacterDescription> DescriptionCharacter { get => _descriptionCharacter; }
        public List<RenPyCommand> Scenario { get => _scenarioParsing; }
        public List<ImagePath> ImagesPath { get => _imagesPath; }

        private List<CharacterDescription> _descriptionCharacter = new List<CharacterDescription>();
        private List<RenPyCommand> _scenarioParsing = new();
        private List<ImagePath> _imagesPath = new List<ImagePath>();

        public ParsingScenarioDeprecated(string scenario)
        {
            _descriptionCharacter = GetCharacterDescriptions(scenario);
            _imagesPath = GetImages(scenario, ref _scenarioParsing);

            _scenarioParsing.AddRange(GetScenario(scenario, _descriptionCharacter));
            _scenarioParsing = _scenarioParsing.OrderBy(a => a.IndexToScenario).ToList();

            Console.WriteLine("parse scenario from message");
        }

        public async Task<string> Parse()
        {
            //await GetImagesFromKadinsky(_imagesPath);
            var result = GetRenPyScript(_scenarioParsing, _imagesPath, _descriptionCharacter);

            await SaveRenPyScript(result, RenPyCodeHelper.RenPyScriptPath);
            Console.WriteLine("generate renpy script");

            return "";
        }

        private Task GetImagesFromKadinsky(List<ImagePath> imagePaths)
        {
            Directory.CreateDirectory(RenPyCodeHelper.BackgroundImagesFolderPath);

            List<Thread> threadsGenerator = new List<Thread>();

            foreach (ImagePath imagePath in imagePaths)
            {
                Thread generator = new Thread(a => { GenerateImage(imagePath, RenPyCodeHelper.BackgroundImagesFolderPath).Wait(); });
                threadsGenerator.Add(generator);
                generator.Start();

                Thread.Sleep(1000);
            }
            foreach (Thread generator in threadsGenerator)
            {
                generator.Join();
            }

            return Task.CompletedTask;
        }

        private async Task SaveRenPyScript(string script, string path)
        {
            await File.WriteAllTextAsync(path, script);
        }

        private async Task<bool> GenerateImage(ImagePath imagePath, string dirPath)
        {
            KadinskyGenerate kadinsky = new KadinskyGenerate("token", "token");
            var model = await kadinsky.GetModel();

            var result = false;

            var uuid = await kadinsky.Generate(imagePath.PromtImage, (int)model, width: 1920, height: 1080);
            if (uuid == null)
                return result;

            string? photo = "";
            for (int i = 0; i < 10 || uuid == photo; i++)
            {
                photo = await kadinsky.CheckGenerate(uuid);

                if (photo is null)
                {
                    return result;
                }

                Thread.Sleep(1000);
            }

            if (photo == uuid)
                return result;

            byte[] data = Convert.FromBase64String(photo);
            File.WriteAllBytes(Path.Combine(dirPath, imagePath.PathToFile), data);
            await Console.Out.WriteLineAsync($"Generate image: {imagePath.PathToFile}");

            return true;
        }

        private string GetRenPyScript(List<RenPyCommand> scenario, List<ImagePath> imagesPath, List<CharacterDescription> characters)
        {
            StringBuilder scriptBuilder = new StringBuilder(RenPyCodeHelper.InitBlock);

            scriptBuilder.AppendLine();

            foreach (var images in imagesPath)
            {
                scriptBuilder.AppendLine(RenPyCodeHelper.GetBackgroundPucture(images.ImageName, images.PathToFile));
            }

            scriptBuilder.AppendLine();

            foreach (var character in characters)
            {
                scriptBuilder.AppendLine(RenPyCodeHelper.GetCharacterInit(character.Name));
            }

            scriptBuilder.AppendLine();

            foreach (var character in characters)
            {
                for (int i = 1; i <= Directory.GetFiles(Path.Combine(RenPyCodeHelper.CharacterImageFolderPathAbsolute, character.Name)).Length; i++)
                {
                    scriptBuilder.AppendLine(RenPyCodeHelper.GetCharacterPicture(character.Name, i.ToString(), i.ToString() + ".png"));
                }
                scriptBuilder.AppendLine();
            }

            scriptBuilder.AppendLine("\nlabel start:\n");

            int position = 0;

            foreach (var command in scenario)
            {
                if(position > 2)
                    position = 0;

                switch (command.Type)
                {
                    case _dalleNameInScenario:
                        scriptBuilder.AppendLine(RenPyCodeHelper.GetShowBackground(command.Command, "with fade"));
                        break;

                    default:
                        var file = (scenario.Where(a => a.Type == command.Type).ToList().FindIndex(a => a.Gesture == command.Gesture) + 1).ToString();

                        scriptBuilder.AppendLine(RenPyCodeHelper.GetShowCharacterEmotion(command.Type, file, $"{_positions[position]}{_moves[position]}"));
                        scriptBuilder.AppendLine(RenPyCodeHelper.GetCahracterVoice(command.Type, file));
                        scriptBuilder.AppendLine(RenPyCodeHelper.GetCharacterPhrase(command.Type, command.Command));
                        position++;
                        break;
                }
            }

            return scriptBuilder.ToString();
        }

        private List<RenPyCommand> GetScenario(string scenario, List<CharacterDescription> characters)
        {
            List<RenPyCommand> renPyScenario = new();
            var lexems = GetLexem(characters);

            int index = scenario.IndexOf(_characterPhrase, 0);
            while (index != -1)
            {
                index += _characterPhrase.Length;
                var command = new RenPyCommand();

                int idexEnd = scenario.IndexOf(_characterPhraseEndInit, index) + _characterPhraseEndInit.Length;
                command.Type = lexems[scenario[(index - _characterPhrase.Length)..idexEnd]];
                command.Command = scenario[idexEnd..scenario.IndexOf("\"", idexEnd)].Replace("\n", "");
                command.Gesture = scenario[(scenario.IndexOf("\"", idexEnd) + 1)..scenario.IndexOf("\n", idexEnd + 1)];
                command.IndexToScenario = index;

                renPyScenario.Add(command);

                index = scenario.IndexOf(_characterPhrase, index + 1);
            }

            return renPyScenario;
        }

        private Dictionary<string, string> GetLexem(List<CharacterDescription> characters)
        {
            var lexems = new Dictionary<string, string>();

            foreach (var item in characters)
            {
                lexems.Add(_characterPhrase + item.Name + _characterPhraseEndInit, item.Name);
            }

            return lexems;
        }

        private List<ImagePath> GetImages(string scenario, ref List<RenPyCommand> commands)
        {
            List<ImagePath> imagePaths = new List<ImagePath>();

            int index = scenario.IndexOf(_dalleLink, 0);
            int counter = 1;
            while (index != -1)
            {
                index += _dalleLink.Length;
                var image = new ImagePath();

                image.PromtImage = scenario[index..scenario.IndexOf("\"", index)];
                image.PathToFile = counter.ToString() + ".jpg";
                image.ImageName += counter.ToString();

                commands.Add(new RenPyCommand() { Type = _dalleNameInScenario, Command = image.ImageName, IndexToScenario = index });

                imagePaths.Add(image);

                counter++;
                index = scenario.IndexOf(_dalleLink, index);
            }

            return imagePaths;
        }

        private List<CharacterDescription> GetCharacterDescriptions(string scenario)
        {
            List<CharacterDescription> descriptionCharacter = new List<CharacterDescription>();

            int index = scenario.IndexOf(_characterInitial, 0);
            while (index != -1)
            {
                index += _characterInitial.Length;
                var character = new CharacterDescription();

                int idexEnd = scenario.IndexOf(_characterFinal, index) + _characterFinal.Length;

                character.Name = scenario[index..(idexEnd - _characterFinal.Length)];
                character.Description = scenario[idexEnd..scenario.IndexOf("\"", idexEnd)];

                descriptionCharacter.Add(character);

                index = scenario.IndexOf(_characterInitial, index + _characterInitial.Length);
            }

            return descriptionCharacter;
        }
    }
}
