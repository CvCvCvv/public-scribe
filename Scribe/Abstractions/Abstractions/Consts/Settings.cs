namespace Application.Abstractions.Consts
{
    public static class Settings
    {
        public static ImageGeneratorProvider BackgroundGenerator { get; } = ImageGeneratorProvider.StableDiffusion;
        public static ImageGeneratorProvider CharacterGenerator { get; } = ImageGeneratorProvider.StableDiffusion;
        public static TTSProvider CharacterVoiceOver { get; } = TTSProvider.Piper;
        public static LLMProvider LLMConnecting { get; } = LLMProvider.Mistral;
        public static bool UseImg2Img { get; } = false;
        public static bool VoiceoverEnabled { get; set; } = false;
        public static bool RemovingBg { get; set; } = false;
        public static bool BranchingDialogues { get; } = false;
        public const string TemplateImage = "tmplt";
    }

    public enum ImageGeneratorProvider
    {
        Kadinsky,
        StableDiffusion
    }

    public enum TTSProvider
    {
        SOVA,
        Piper
    }

    public enum LLMProvider
    {
        ChatGPT,
        Mistral
    }
}
