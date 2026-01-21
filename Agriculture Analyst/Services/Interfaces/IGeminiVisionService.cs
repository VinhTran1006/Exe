namespace Agriculture_Analyst.Services.Interfaces
{
    public interface IGeminiVisionService 
    {
        Task<string> AnalyzeImageAsync(string base64Image, string prompt);

    }
}
