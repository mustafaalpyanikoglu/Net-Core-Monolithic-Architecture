using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebAPI.Application.Pipelines.Logging;
using WebAPI.Application.Rules;
using WebAPI.CrossCuttingConcerns;
using WebAPI.CrossCuttingConcerns.Logging.Serilog.Logger;
using WebAPI.Security.EmailAuthenticator;
using WebAPI.Security.Jwt;
using WebAPI.Security.OtpAuthenticator.OtpNet;
using WebAPI.Security.OtpAuthenticator;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WebAPI.Contexts;
using WebAPI.Repositories.Abstract;
using WebAPI.Repositories.Concrete;
using BusinessLayer.Services.AuthService;
using BusinessLayer.Services.ImageService;
using BusinessLayer.Services.UserService;
using Infrastructure.Adapters.ImageService;
using Core.Mailing.MailKitImplementations;
using Core.Mailing;

namespace BusinessLayer;

public static class WebApiServiceRegistration
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ImageServiceBase, CloudinaryImageServiceAdapter>();
        services.AddScoped<IAuthService, AuthManager>();
        services.AddScoped<IUserService, UserManager>();

        services.AddSingleton<LoggerServiceBase, MsSqlLogger>();
        //services.AddSingleton<LoggerServiceBase, FileLogger>();

        services.AddSingleton<IMailService, MailKitMailService>();

        #region Security
        services.AddScoped<ITokenHelper, JwtHelper>();
        services.AddScoped<IEmailAuthenticatorHelper, EmailAuthenticatorHelper>();
        services.AddScoped<IOtpAuthenticatorHelper, OtpNetOtpAuthenticatorHelper>();
        #endregion

        #region Adds data access layer services to the IServiceCollection.
        services.AddDbContext<BaseDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DBConnectionString"));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerWarehouseCostRepository, CustomerWarehouseCostRepository>();
        services.AddScoped<IEmailAuthenticatorRepository, EmailAuthenticatorRepository>();
        services.AddScoped<IOtpAuthenticatorRepository, OtpAuthenticatorRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        #endregion

        return services;
    }

    public static IServiceCollection AddSubClassesOfType(
    this IServiceCollection services,
    Assembly assembly,
    Type type,
    Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (var item in types)
        {
            if (addWithLifeCycle == null)
            {
                services.AddScoped(item);
            }
            else
            {
                addWithLifeCycle(services, type);
            }
        }
        return services;
    }
}
