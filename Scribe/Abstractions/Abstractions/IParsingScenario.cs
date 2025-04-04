namespace ParsingScenario.Abstractions
{
    public interface IParsingScenario
    {
        public Task<string> Parse();
    }
}
