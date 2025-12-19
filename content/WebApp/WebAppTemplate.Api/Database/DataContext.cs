using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebAppTemplate.Api.Configuration;
using WebAppTemplate.Api.Database.Entities;

namespace WebAppTemplate.Api.Database;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    private readonly IOptions<DatabaseOptions> Options;

    public DataContext(IOptions<DatabaseOptions> options)
    {
        Options = options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseNpgsql(
            $"Host={Options.Value.Host};" +
            $"Port={Options.Value.Port};" +
            $"Username={Options.Value.Username};" +
            $"Password={Options.Value.Password};" +
            $"Database={Options.Value.Database}"
        );
    }
}