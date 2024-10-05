using Microsoft.EntityFrameworkCore;
using web_api.Entities;

namespace web_api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Test>().HasKey(t => t.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
    }
    public DbSet<Test> Tests { get; set; }
    public DbSet<User> Users { get; set; }
}