namespace Application.Abstractions.Domains
{
    public class RenPyCommand
    {
        public string Type { get; set; } = null!;
        public string Command { get; set; } = null!;
        /// <summary>
        /// Мимика (необязательна) 
        /// </summary>
        public string? Gesture { get; set; }
        public int IndexToScenario { get; set; } = -1;
    }
}