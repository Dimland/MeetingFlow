namespace RegistrationsManager.Messaging;

public record RegistrationCreatedEvent(
    Guid RegistrationId,
    Guid MeetingId,
    Guid AttendeeId,
    string MeetingTitle,
    string AttendeeEmail,
    DateTimeOffset RegisteredAt);
