using System.ComponentModel.DataAnnotations;

namespace WebAppTemplate.Shared.Http.Requests.Users;

public class UpdateUserDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    public string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}