namespace Core.Dtos;

public record UserCreatedEvent
(
    int UserId,
    string Name,
    string Email
);