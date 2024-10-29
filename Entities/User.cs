using System.ComponentModel.DataAnnotations;

namespace web_api.Entities;

public class User : BaseEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    [MaxLength(128)]  public required string FirstName { get; set; }
    [MaxLength(128)] public required string LastName { get; set; }
    [MaxLength(256)] public required string Email { get; set; }
    [MaxLength(2048)] public string ProfilePicture { get; set; } = string.Empty;
}