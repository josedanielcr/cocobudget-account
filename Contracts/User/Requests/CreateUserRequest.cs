using System.ComponentModel.DataAnnotations;

namespace web_api.Contracts.User.Requests;

public class CreateUserRequest
{
    [Required]
    [MaxLength(128)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(128)]
    public string LastName { get; set; } = null!;

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [MaxLength(2048)]
    public string? ProfilePicture { get; set; }
}
