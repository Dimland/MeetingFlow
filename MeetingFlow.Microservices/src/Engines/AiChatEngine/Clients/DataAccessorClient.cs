using System.Net.Http.Json;
using System.Text.Json;

namespace AiChatEngine.Clients;

public class DataAccessorClient(HttpClient http)
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<MeetingSummary>> GetMeetingsAsync()
        => await http.GetFromJsonAsync<List<MeetingSummary>>("/data/meetings", JsonOpts) ?? [];

    public async Task<MeetingSummary?> GetMeetingAsync(Guid id)
        => await http.GetFromJsonAsync<MeetingSummary>($"/data/meetings/{id}", JsonOpts);

    public async Task<List<TaskItem>> GetTasksAsync()
        => await http.GetFromJsonAsync<List<TaskItem>>("/data/tasks", JsonOpts) ?? [];

    public async Task<List<TaskItem>> GetTasksByMeetingAsync(Guid meetingId)
        => await http.GetFromJsonAsync<List<TaskItem>>($"/data/tasks/by-meeting/{meetingId}", JsonOpts) ?? [];

    public async Task<TaskItem?> CreateTaskAsync(TaskItem task)
    {
        var resp = await http.PostAsJsonAsync("/data/tasks", task);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<TaskItem>(JsonOpts) : null;
    }

    public async Task<TaskItem?> CompleteTaskAsync(Guid taskId)
    {
        var existing = await http.GetFromJsonAsync<TaskItem>($"/data/tasks/{taskId}", JsonOpts);
        if (existing is null) return null;
        existing.IsCompleted = true;
        var resp = await http.PutAsJsonAsync($"/data/tasks/{taskId}", existing);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<TaskItem>(JsonOpts) : null;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var resp = await http.DeleteAsync($"/data/tasks/{taskId}");
        return resp.IsSuccessStatusCode;
    }
}

public class MeetingSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
}

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? AssignedTo { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
