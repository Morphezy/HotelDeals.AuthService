using Application.Repositories;
using Application.Services;
using Infrastructure.Repositories;
using Web.Services;

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
        services.AddScoped<ITokenService, JwtService>();
        
        services.AddControllers();
    }
}
