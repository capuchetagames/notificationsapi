namespace Core.Entity;

public class Notifications : EntityBase
{
    public required int UserId { get; set; }
    public required string Message { get; set; }
    public required string Subject { get; set; }
    
    public required string Type { get; set; }
    public required string Status { get; set; }
    
    public DateTime DeliveredAt { get; set; }
    
}