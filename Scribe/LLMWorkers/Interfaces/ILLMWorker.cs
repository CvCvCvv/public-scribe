namespace LLMWorkers.Interfaces
{
    public interface ILLMWorker
    {
        public Task<string> GetScenario(string theme);
    }
}
