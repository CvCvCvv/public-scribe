namespace ImageGenerators.Interfaces
{
    public interface IGeneratorImage
    {
        public Task<(string?, bool)> GenerateImageToBase64(string prompt, int width, int height, string style = "UHD");
        public Task<(string?, bool)> GenerateImage2ImageToBase64(string prompt, string image_b64, int width, int height, double strength = 0.75);
    }
}
