namespace API.Extensions;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // Add services to the container.
        services.AddControllers();
        services.AddDbContext<DataContext>(opt => opt.UseSqlite(config.GetConnectionString("DefaultConnection")));
        services.AddCors();

        // En donde encuentre 'ITokenService' lo remplaza por (injecta) 'TokenService'
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
