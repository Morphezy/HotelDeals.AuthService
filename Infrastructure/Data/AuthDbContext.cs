using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AuthDbContext :DbContext
{
public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options){}

public DbSet<Registration> Registrations { get; set; }
public DbSet<User> Users { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    
}
}