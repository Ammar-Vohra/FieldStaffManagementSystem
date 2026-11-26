using FieldStaff.Application.Features.Chat.DTOs;
using FieldStaff.Application.Features.Chat.Interfaces;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FieldStaff.Infrastructure.ExternalServices.Groq
{
    public class GroqChatClient :  IGroqChatClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration; 

        public GroqChatClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

        }

        public async Task<GroqChatCompletionResponseDto> GetCompletionAsync(
            List<GroqChatMessageDto> messages, object[] tools)
        {
            var apiKey = _configuration["Groq:ApiKey"];
            var model = _configuration["Groq:Model"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "Groq API Key is not configured.");

            if (string.IsNullOrWhiteSpace(model))
                throw new InvalidOperationException(
                    "Groq model is not configured.");

            var requestBody = new
            {
                model,
                messages,
                tools,
                tool_choice = "auto",
                temperature = 0.3,
                max_tokens = 1024
            };

            var json = JsonSerializer.Serialize(
                requestBody,
                new JsonSerializerOptions(
                    JsonSerializerDefaults.Web));

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.groq.com/openai/v1/chat/completions");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Groq API returned {(int)response.StatusCode}: " +
                    responseContent);
            }

            using var document = JsonDocument.Parse(responseContent);

            var choice = document.RootElement.GetProperty("choices")[0];
            var message = choice.GetProperty("message");

            var result = new GroqChatCompletionResponseDto
            {
                FinishReason = choice.GetProperty("finish_reason").GetString(),
                Content = message.TryGetProperty(
                    "content", out var contentElement)
                    && contentElement.ValueKind == JsonValueKind.String
                        ? contentElement.GetString()
                        : null
            };

            if (message.TryGetProperty(
                "tool_calls", out var toolCallsElement))
            {
                foreach (var toolCall in toolCallsElement.EnumerateArray())
                {
                    var function = toolCall.GetProperty("function");

                    result.ToolCalls.Add(new GroqToolCallDto
                    {
                        Id = toolCall.GetProperty("id").GetString()
                            ?? string.Empty,

                        Type = toolCall.GetProperty("type").GetString()
                            ?? "function",

                        Function = new GroqFunctionCallDto
                        {
                            Name = function.GetProperty("name").GetString()
                                ?? string.Empty,

                            Arguments = function
                                .GetProperty("arguments").GetString()
                                ?? "{}"
                        }
                    });
                }
            }


            return result;

        }
    }
}
