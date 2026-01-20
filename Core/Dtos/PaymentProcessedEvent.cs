namespace Core.Dtos;

public record PaymentProcessedEvent
(
    int UserId,
    string Email,
    string Name,
    string Status
);