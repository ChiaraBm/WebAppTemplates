namespace WebAppTemplate.Api.Database.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; }
    public string Email { get; set; }

    public DateTimeOffset InvalidateTimestamp { get; set; }
}