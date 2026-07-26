namespace Vesta.Application.DTOs;

public class UserDto
{
    public Guid? Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public IList<string> Roles { get; set; } = [];

    public DateTime CreatedAt { get; set; }
}