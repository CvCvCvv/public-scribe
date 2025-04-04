namespace Application.Abstractions.Domains
{
    public class CharacterPhrases
    {
        public string Name { get; set; } = null!;
        public List<string> Phrases { get; set; } = new();
    }
}
