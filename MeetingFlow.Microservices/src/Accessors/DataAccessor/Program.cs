using DataAccessor.Data;
using DataAccessor.Models;
using DataAccessor.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration["POSTGRES_CONN"]
           ?? Environment.GetEnvironmentVariable("POSTGRES_CONN")
           ?? "Host=localhost;Port=5432;Database=meetingflow;Username=meetingflow;Password=meetingflow";

builder.Services.AddDbContext<MeetingFlowDbContext>(o => o.UseNpgsql(conn));
builder.Services.AddScoped<MeetingsRepository>();
builder.Services.AddScoped<RegistrationsRepository>();
builder.Services.AddScoped<FeedbackRepository>();
builder.Services.AddScoped<MeetingTasksRepository>();

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MeetingFlowDbContext>();
    for (var i = 0; i < 20; i++)
    {
        try
        {
            db.Database.EnsureCreated();
            var creator = db.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
            try { creator.CreateTables(); } catch { /* Tables may already exist */ }
            break;
        }
        catch { Thread.Sleep(1500); }
    }
    SeedData.Initialize(db);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "DataAccessor" }));

// Meetings repository endpoints — return full entity graphs on purpose.
app.MapGet("/data/meetings", async (MeetingsRepository r) => Results.Ok(await r.GetAllAsync()));
app.MapGet("/data/meetings/{id:guid}", async (Guid id, MeetingsRepository r) =>
    await r.GetByIdAsync(id) is { } m ? Results.Ok(m) : Results.NotFound());
app.MapPut("/data/meetings/{id:guid}", async (Guid id, Meeting body, MeetingsRepository r) =>
{
    body.Id = id;
    var saved = await r.UpsertAsync(body);
    return Results.Ok(saved);
});
app.MapGet("/data/meetings/{id:guid}/sessions", async (Guid id, MeetingsRepository r) =>
    Results.Ok(await r.GetSessionsByMeetingAsync(id)));

// Speakers
app.MapGet("/data/speakers", async (MeetingsRepository r) => Results.Ok(await r.GetAllSpeakersAsync()));
app.MapGet("/data/speakers/{id:guid}", async (Guid id, MeetingsRepository r) =>
    await r.GetSpeakerByIdAsync(id) is { } s ? Results.Ok(s) : Results.NotFound());

// Registrations repository endpoints.
app.MapGet("/data/registrations", async (RegistrationsRepository r) => Results.Ok(await r.GetAllAsync()));
app.MapGet("/data/registrations/by-meeting/{meetingId:guid}", async (Guid meetingId, RegistrationsRepository r) =>
    Results.Ok(await r.GetByMeetingAsync(meetingId)));
app.MapPost("/data/registrations", async (Registration body, RegistrationsRepository r) =>
{
    var saved = await r.CreateAsync(body);
    return Results.Created($"/data/registrations/{saved.Id}", saved);
});

app.MapGet("/data/attendees", async (RegistrationsRepository r) => Results.Ok(await r.GetAllAttendeesAsync()));
app.MapGet("/data/attendees/{id:guid}", async (Guid id, RegistrationsRepository r) =>
    await r.GetAttendeeAsync(id) is { } a ? Results.Ok(a) : Results.NotFound());

// Feedback repository endpoints.
app.MapGet("/data/feedback/by-meeting/{meetingId:guid}", async (Guid meetingId, FeedbackRepository r) =>
    Results.Ok(await r.GetByMeetingAsync(meetingId)));
app.MapPost("/data/feedback", async (Feedback body, FeedbackRepository r) =>
{
    var saved = await r.CreateAsync(body);
    return Results.Created($"/data/feedback/{saved.Id}", saved);
});

// --- Meeting Tasks (action items) ---
app.MapGet("/data/tasks", async (MeetingTasksRepository r) => Results.Ok(await r.GetAllAsync()));
app.MapGet("/data/tasks/by-meeting/{meetingId:guid}", async (Guid meetingId, MeetingTasksRepository r) =>
    Results.Ok(await r.GetByMeetingAsync(meetingId)));
app.MapGet("/data/tasks/{id:guid}", async (Guid id, MeetingTasksRepository r) =>
    await r.GetByIdAsync(id) is { } t ? Results.Ok(t) : Results.NotFound());
app.MapPost("/data/tasks", async (MeetingTask body, MeetingTasksRepository r) =>
{
    var saved = await r.CreateAsync(body);
    return Results.Created($"/data/tasks/{saved.Id}", saved);
});
app.MapPut("/data/tasks/{id:guid}", async (Guid id, MeetingTask body, MeetingTasksRepository r) =>
    await r.UpdateAsync(id, body) is { } t ? Results.Ok(t) : Results.NotFound());
app.MapDelete("/data/tasks/{id:guid}", async (Guid id, MeetingTasksRepository r) =>
    await r.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());

app.Run();
