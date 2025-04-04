namespace TextToSpeech.Interfaces
{
    public interface ITTSWorker
    {
        public Task<(string?, bool)> ToSpeech(string text, string voicebank); 
    }
}
