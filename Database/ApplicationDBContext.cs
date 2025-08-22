using Microsoft.EntityFrameworkCore;

namespace DataVizManager.Database;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; }
}

public class User
{
  public int Id { get; set; }
  public string Email { get; set; }

  public string UserName { get; set; }

  public string PasswordHash { get; set; }
  public string Role { get; set; } = "user";
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
