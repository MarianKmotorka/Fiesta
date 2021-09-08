using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Fiesta.Infrastracture.Auth;
using Fiesta.Infrastracture.Messaging.Email;
using Fiesta.Infrastracture.Persistence;
using Fiesta.Infrastracture.Resources.Images;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace Fiesta.Infrastracture.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FiestaDbContext>(options =>
                 options.UseSqlServer(
                     configuration.GetConnectionString("FiestaDb"),
                     builder => builder.MigrationsAssembly(typeof(FiestaDbContext).Assembly.FullName)));

            services.AddIdentity<AuthUser, AuthRole>(o =>
            {
                o.Password.RequiredLength = 6;
                o.Password.RequireDigit = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.User.RequireUniqueEmail = true;
                o.User.AllowedUserNameCharacters = Application.Common.Validators.Helpers.UsernameAllowedCharacters;
            })
               .AddRoles<AuthRole>()
               .AddEntityFrameworkStores<FiestaDbContext>()
               .AddDefaultTokenProviders();

            services.AddJwtAuthentication(configuration);
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFiestaDbContext, FiestaDbContext>();
            services.AddScoped<IGoogleService, GoogleService>();

            var emailOptions = new EmailOptions();
            configuration.GetSection(nameof(EmailOptions)).Bind(emailOptions);

            services
            .AddFluentEmail(emailOptions.Email)
            .AddRazorRenderer()
            .AddSmtpSender(
                new SmtpClient(emailOptions.Host, emailOptions.Port)
                { Credentials = new NetworkCredential("apikey", emailOptions.Password), EnableSsl = true }
                );

            services.AddTransient<IEmailService, EmailService>();

            var webClientOptions = new WebClientOptions();
            configuration.GetSection(nameof(WebClientOptions)).Bind(webClientOptions);
            services.AddSingleton(webClientOptions);

            var cloudinaryOptions = new CloudinaryOptions();
            configuration.GetSection(nameof(CloudinaryOptions)).Bind(cloudinaryOptions);
            services.AddSingleton(cloudinaryOptions);

            services.AddScoped<IImageService, CloudinaryService>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
