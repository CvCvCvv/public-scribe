using Application.Abstractions.Domains;
using ParsingScenario.Abstractions;

namespace ParsingScenario
{
    public class ParsingScenario : IParsingScenario
    {
        // Маячки для парсера
        private const string _dalleLink = "{DALL-E}: \"";
        private const string _characterInitial = "<characters";
        private const string _characterFinal = ">";
        private const string _imageCharacter = "[sprite:";
        private const string _imageBackground = "[back-image_";
        private const string _endImage = "]";
        private const string _characterPhraseEndInit = "]: \"";
        private const string _renpyStart = "```renpy";
        private const string _renpyEnd = "```";
        private const string _scriptStart = "label start:";
        //
        private List<string> _namesCharacters = new List<string>();
        private string _scenario = "";
        private string _script = "";

        public List<ImagePath> ImagesPath { get; set; } = new List<ImagePath>();
        public List<ImagePath> CharacterImagesPath { get; set; } = new List<ImagePath>();
        public List<CharacterPhrases> CharacterPhrases { get; set; } = new();


        public ParsingScenario(string scenario)
        {
            _scenario = scenario;
            _script = GetRenPyScript(_scenario);

            GetNames(_scenario);
            GetBackgroundsImages(_scenario);
            GetCharacterImages(_scenario);
            GetCharacterPhrases(_script);
        }

        public async Task<string> Parse()
        {
            await SaveRenPyScript(_script, RenPyCodeHelper.RenPyScriptPath);
            Console.WriteLine("generate renpy script");

            return "";
        }

        private string GetRenPyScript(string scenario)
        {
            var start = scenario.IndexOf(_renpyStart) + _renpyStart.Length;
            var end = scenario.IndexOf(_renpyEnd, start);
            return ModifyScript(scenario[start..end]);
        }

        private string ModifyScript(string script)
        {
            return script.Replace("    return", "    $ quit()");
        }

        private async Task SaveRenPyScript(string script, string path)
        {
            await File.WriteAllTextAsync(path, script);
        }

        private void GetBackgroundsImages(string scenario)
        {
          
            int index = scenario.IndexOf(_imageBackground, 0);
            while (index > -1)
            {
                index++;
                var endIndexName = scenario.IndexOf(_endImage, index);
                var startPrompt = scenario.IndexOf("\"", endIndexName + 2) + 1;
                var endPrompt = scenario.IndexOf("\"", startPrompt);

                ImagesPath.Add(new ImagePath() { 
                    ImageName = scenario[index..endIndexName],
                    PromtImage = scenario[ startPrompt..endPrompt],
                    PathToFile = scenario[index..endIndexName] + ".jpg"
                });

                index = scenario.IndexOf(_imageBackground, endPrompt);
            }
        }

        private void GetCharacterPhrases(string script)
        {
            foreach (var name in _namesCharacters)
            {
                var findName = name + " \"" ;
                var character = new CharacterPhrases() { Name = name };

                int index = script.IndexOf(findName, 0);
                while (index > -1)
                {
                    index+= findName.Length;
                    var endPhrase = script.IndexOf("\"", index);

                    character.Phrases.Add(script[index .. endPhrase]);

                    index = script.IndexOf(findName, endPhrase);
                }

                CharacterPhrases.Add(character);
            }
        }

        private void GetCharacterImages(string scenario)
        {
            foreach (var name in _namesCharacters)
            {
                int index = scenario.IndexOf(_imageCharacter + name, 0);
                while (index > -1)
                {
                    index++;
                    var endIndexName = scenario.IndexOf(_endImage, index);
                    var startPrompt = scenario.IndexOf("\"", endIndexName + 2) + 1;
                    var endPrompt = scenario.IndexOf("\"", startPrompt);

                    CharacterImagesPath.Add(new ImagePath()
                    {
                        ImageName = scenario[index..endIndexName].Replace("sprite:", ""),
                        PromtImage = scenario[startPrompt..endPrompt],
                        PathToFile = Path.Combine(name, scenario[index..endIndexName].Replace("sprite:", "") + ".jpg")
                    });

                    index = scenario.IndexOf(_imageCharacter + name, endPrompt);
                }
            }
        }

        private void GetNames(string scenario)
        {
            int start = scenario.IndexOf(_characterInitial) + _characterInitial.Length;
            int end = scenario.IndexOf(_characterFinal);
            var names = scenario[start..end].Replace("\r", "").Replace("\n", "").Trim();

            _namesCharacters = names.Split(" ").ToList();
        }
    }
}
