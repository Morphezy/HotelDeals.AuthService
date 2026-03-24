using Application.Repositories;
using Infrastructure.Repositories;

namespace Web;

public static class DependencyInjection
{
    public static void AddDependencyInjection(this IServiceCollection services)
    {
        services.InitServices();
    }

    public static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        
        
        services.AddControllers();
    }
}