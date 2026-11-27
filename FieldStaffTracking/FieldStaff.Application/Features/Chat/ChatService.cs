
using System.Text.Json;
using FieldStaff.Application.Features.Chat.DTOs;
using FieldStaff.Application.Features.Chat.Interfaces;

namespace FieldStaff.Application.Features.Chat
{
    public class ChatService : IChatService
    {
        private readonly IGroqChatClient _groqChatClient;
        private readonly IChatToolService _chatToolService;

        private const int MaxToolRounds = 3;

        public ChatService(
            IGroqChatClient groqChatClient,
            IChatToolService chatToolService)
        {
            _groqChatClient = groqChatClient;
            _chatToolService = chatToolService;
        }

        private const string SystemPrompt = @"

You are the AI Operational Assistant for a Field Staff
Tracking and Management System.

Your responsibilities are:
1. Help users understand operational task information.
2. Answer questions about task statistics.
3. Explain staff workload.
4. Identify overdue tasks.
5. Summarize operational activities.

Rules:
- Always use available tools when answering questions
  that require actual application data.
- Never invent task counts, staff names, or statistics.
- Never assume information that is not provided.
- Do not generate or execute SQL queries.
- Do not perform database modifications.
- Do not assign or reassign service tasks.
- Do not change task statuses.
- Do not expose information outside the user's
  authorized access.
- If information is unavailable, clearly explain that.
- Present responses in simple, professional language.

Response Formatting Rules:
- Never use Markdown tables.
- Use bullet points for short lists and statistics.
- Use numbered lists when displaying multiple service tasks.
- Keep responses organized and easy to read.
- Use bold text for task titles and important labels.
- Separate individual tasks with a blank line.
- Avoid unnecessarily long paragraphs.
- Do not repeat information unnecessarily.
- Do not include fields that are unavailable in the
  tool response.
- Do not invent additional task details.
- When no records are found, clearly communicate that.

When displaying service tasks, use this format:

**1. Task Title**

- Task ID: [Task ID]
- Customer: [Customer Name]
- Assigned Staff: [Staff Name]
- Status: [Task Status]
- Priority: [Priority]
- Scheduled Date: [Scheduled Date]

For a single task, use the same format without numbering.

For task statistics, use this format:

**Task Summary**
- Total Tasks: [Count]
- Pending: [Count]
- In Progress: [Count]
- Completed: [Count]
- Overdue: [Count]

Only include statistics supported by tool results.

You are an assistant, not an administrator.
All actual operations must be performed through
the application's authorized interfaces."
;

        private static readonly object[] Tools =
        {
            new
            {
                type = "function",
                function = new
                {
                    name = "GetDashboardSummary",
                    description = "Retrieves the authenticated user's role-specific dashboard summary.",
                    parameters = new
                    {
                        type = "object",
                        properties = new { },
                        required = Array.Empty<string>()
                    }
                }
            },


            new
            {
                type = "function",
                function = new
                {
                    name = "GetOverdueTasks",
                    description = "Retrieves overdue service tasks visible to the current user. Use this when the user asks about overdue, late, or past-due tasks.",
                    parameters = new
                    {
                        type = "object",
                        properties = new { },
                        required = Array.Empty<string>()
                    }
                }
            },

            new
            {
                type = "function",
                function = new
                {
                    name = "GetServiceTasks",
                    description = "Retrieves service tasks visible to the authenticated user. Use this for searching tasks or retrieving tasks scheduled within a date range.",
                    parameters = new
                    {
                        type = "object",
                        properties = new
                        { 
                            search = new
                            {
                                type = "string",
                                description = "Search term for task title or description, customer name, or staff name."
                            }

                        },


                        required = Array.Empty<string>()
                    }
                }
            }
        };

        public async Task<ChatResponseDto> SendMessageAsync(
            ChatRequestDto request,
            int roleId,
            int userId)
        {
            var messages = new List<GroqChatMessageDto>
            {
                new()
                {
                    Role = "system",
                    Content = SystemPrompt
                }
            };

            if (request.History != null)
            {
                foreach (var historyMessage in request.History.TakeLast(10))
                {
                    var role = historyMessage.Role?.Trim().ToLowerInvariant();

                    if (role != "user" && role != "assistant")
                        continue;

                    if (string.IsNullOrWhiteSpace(historyMessage.Content))
                        continue;

                    messages.Add(new GroqChatMessageDto
                    {
                        Role = role,
                        Content = historyMessage.Content

                    });
                }
            }

            messages.Add(new GroqChatMessageDto
            {
                Role = "user",
                Content = request.Message

             });

            for (int round = 0; round < MaxToolRounds; round++)
            {
                var groqResponse =
                    await _groqChatClient.GetCompletionAsync(
                        messages,
                        Tools);

                // Groq has finished generating a normal response.
                if (groqResponse.FinishReason != "tool_calls" ||
                    groqResponse.ToolCalls.Count == 0)
                {
                    return new ChatResponseDto
                    {
                        Reply = groqResponse.Content
                            ?? "I couldn't generate a response."
                    };
                }

                // Add Groq's tool-call message to conversation history.
                messages.Add(new GroqChatMessageDto
                {
                    Role = "assistant",
                    Content = groqResponse.Content,
                    ToolCalls = groqResponse.ToolCalls
                });

                // Execute the requested backend tools.
                foreach (var toolCall in groqResponse.ToolCalls)
                {
                    string toolResult;

                    if (toolCall.Function.Name == "GetDashboardSummary")
                    {
                        var summary =
                            await _chatToolService.GetDashboardSummaryAsync(
                                roleId,
                                userId);

                        toolResult = JsonSerializer.Serialize(summary);
                    }

                    else if (toolCall.Function.Name == "GetOverdueTasks")
                    {
                        var result = await _chatToolService.GetOverdueTasksAsync(
                            roleId,
                            userId
                        );

                        toolResult = JsonSerializer.Serialize(result);
                    }

                    else if (toolCall.Function.Name == "GetServiceTasks")
                    {
                        using var document = JsonDocument.Parse(
                            string.IsNullOrWhiteSpace(toolCall.Function.Arguments)
                            ? "{}"
                            : toolCall.Function.Arguments);

                        var root = document.RootElement;

                        string? search = null; 

                        if (root.TryGetProperty("search", out var searchElement) &&
                            searchElement.ValueKind == JsonValueKind.String)
                        {
                            search = searchElement.GetString();
                        }


                        var result = await _chatToolService.GetServiceTaskAsync(
                            roleId, userId, search);

                        toolResult = JsonSerializer.Serialize(result);
                    }

                    else
                    {
                        toolResult = "Error: This tool is not available.";
                    }

                    // Return the tool result to Groq.
                    messages.Add(new GroqChatMessageDto
                    {
                        Role = "tool",
                        ToolCallId = toolCall.Id,
                        Content = toolResult
                    });
                }
            }

            return new ChatResponseDto
            {
                Reply = "I couldn't complete your request. Please try again."
            };
        }
    }
}
