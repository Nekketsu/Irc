using Irc.Bot.Console.Models;
using System.Net.Http.Json;

namespace Irc.Bot.Console
{
    public class OpenAiService
    {
        public string ApiKey { get; }

        public OpenAiService(string apiKey)
        {
            ApiKey = apiKey;
        }

        public async Task<string> PostCompletionsAsync(string prompt)
        {
            var requestUri = "https://api.openai.com/v1/completions";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var request = new CompletionsRequest
            {
                model = "text-davinci-003",
                prompt = prompt,
                max_tokens = 2048,
                temperature = 0.5,
                n = 1
            };

            var response = await httpClient.PostAsJsonAsync(requestUri, request);
            var completionsResponse = await response.Content.ReadFromJsonAsync<CompletionsResponse>();

            var responseText = completionsResponse.choices.FirstOrDefault()?.text.Trim();

            return responseText;
        }
    }
}
