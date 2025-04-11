using UserStoryAnalyzer.Models;

namespace OpenAI_ChatGPT
{
    public interface IChatCompletionService
    {
        Task<string> GetChatCompletionAsync(List<UserStory> userStories);
    }
}