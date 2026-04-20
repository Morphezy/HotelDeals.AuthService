using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "../Web");
        
        IConfigurationRoot configurationRoot = new ConfigurationBuilder().SetBasePath(path).AddJsonFile("appsettings.json").Build();
        
        var connectionString = configurationRoot.GetConnectionString("AuthDb");
        
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
            
        return new AuthDbContext(optionsBuilder.Options);
    }
}