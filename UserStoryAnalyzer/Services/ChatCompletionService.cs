using System.Text;
using System.Text.Json;
using UserStoryAnalyzer.Models;

namespace OpenAI_ChatGPT.Services
{
    public class ChatCompletionService : IChatCompletionService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatCompletionService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetChatCompletionAsync(List<UserStory> userStories)
        {
            var httpClient = _httpClientFactory.CreateClient("ChatGPT");
            var responses = new List<string>();

            // Split user stories into batches
            const int batchSize = 10; // Adjust batch size as needed
            for (int i = 0; i < userStories.Count; i += batchSize)
            {
                var batch = userStories.Skip(i).Take(batchSize).ToList();
                var question = $"Determine if there is interdependency between the user stories by NLP algorithms, cosine, text similarities, etc. {JsonSerializer.Serialize(batch)} give it in a graph.";

                ChatCompletionRequest completionRequest = new()
                {
                    Model = "gpt-4-turbo-2024-04-09",
                    MaxTokens = 1000, // Adjust max tokens as needed
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Role = "user",
                            Content = question,
                        }
                    }
                };

                using var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://userstory-impact-detection.openai.azure.com/openai/deployments/gpt-4/chat/completions?api-version=2025-01-01-preview");
                httpReq.Headers.Add("Authorization", $"{_configuration["OpenAIKey"]}");

                string requestString = JsonSerializer.Serialize(completionRequest);
                httpReq.Content = new StringContent(requestString, Encoding.UTF8, "application/json");

                using HttpResponseMessage? httpResponse = await httpClient.SendAsync(httpReq);
                httpResponse.EnsureSuccessStatusCode();

                var completionResponse = httpResponse.IsSuccessStatusCode ? JsonSerializer.Deserialize<ChatCompletionResponse>(await httpResponse.Content.ReadAsStringAsync()) : null;
                if (completionResponse?.Choices?[0]?.Message?.Content != null)
                {
                    responses.Add(completionResponse.Choices[0].Message.Content);
                }
            }

            return string.Join("\n", responses);
        }
    }
}
