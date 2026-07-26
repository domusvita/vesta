namespace Vesta.Domain.Entities;

public class NotificationEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string? Payload { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserEntity User { get; set; } = null!;
}