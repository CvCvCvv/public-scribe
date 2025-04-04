namespace Application.Abstractions.Domains.Models.GenerateStory
{
    public class StreamerStoryInfoModel
    {
        public Guid Id { get; set; }
        public GenerateStoryModel Story { get; set; } = null!;
    }
}
