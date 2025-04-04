using Application.Abstractions.Consts;
using Application.Abstractions.Domains;
using TextToSpeech.Interfaces;
using TextToSpeech.TTSWorkers;

namespace ParsingScenario.Services
{
    public class VoiceGeneratorService
    {
        private readonly string _generatorUrl;
        private readonly ITTSWorker _ttsWorker;
        public VoiceGeneratorService(string url)
        {
            _generatorUrl = url;
            switch (Settings.CharacterVoiceOver)
            {
                case TTSProvider.SOVA:
                    _ttsWorker = new SovaWorker(_generatorUrl);
                    break;
                case TTSProvider.Piper:
                    _ttsWorker = new PiperWorker(_generatorUrl);
                    break;
                default:
                    _ttsWorker = new SovaWorker(_generatorUrl);
                    break;
            }
        }

        public async Task Generating(List<RenPyCommand> scenario)
        {
            if (Settings.VoiceoverEnabled)
                foreach (var command in scenario)
                {
                    if (command.Gesture is not null)
                    {
                        var filepath = CreateFile(command.Type);
                        await Generate(command.Command, filepath, CheckGender(command.Type));
                    }
                }
        }

        public async Task Generating(List<CharacterPhrases> scenario)
        {
            if (Settings.VoiceoverEnabled)
                foreach (var command in scenario)
                {
                    foreach (var phrase in command.Phrases)
                    {
                        var filepath = CreateFile(command.Name);
                        await Generate(phrase, filepath, CheckGender(command.Name));
                    }
                }
        }

        private async Task Generate(string text, string filepath, string voice = "Natasha")
        {
            var (speech, isSuccess) = await _ttsWorker.ToSpeech(text, voice);

            if (!isSuccess)
            {
                speech = "";
                //TODO Какой нибудь шаблон на голос
            }

            await SaveFile(filepath, speech!);
        }

        private async Task SaveFile(string filepath, string file)
        {
            byte[] data = Convert.FromBase64String(file);
            await File.WriteAllBytesAsync(filepath, data);
            await Console.Out.WriteLineAsync($"Generate voice character: {filepath}");
        }

        private string CreateFile(string name)
        {
            var path = Path.Combine(RenPyCodeHelper.CharacterVoiceFolderPathAbsolute, name);
            Directory.CreateDirectory(path);

            var filename = (Directory.GetFiles(path).Length + 1).ToString() + ".wav";
            var filepath = Path.Combine(path, filename);

            using var a = File.Create(filepath);

            return filepath;
        }

        private string CheckGender(string name)
        {
            return (name[name.Length - 1] == 'а' || name[name.Length - 1] == 'я') ? "woman" : "man";
        }
    }
}
