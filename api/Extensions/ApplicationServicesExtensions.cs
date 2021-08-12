using System.Text;
using api.Context;
using api.Helpers;
using api.Repository;
using api.Repository.Interfaces;
using api.Services;
using api.Services.Interfaces;
using api.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace api.Extentions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){

            
            services.AddDbContext<DataContext>(options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });  

            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped<IPhotoService,PhotoService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); 
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddScoped<LogUserActivity>();
            // Use AddSingleton porque todo mundo que se conectar vai usar a mesma intancia do PresenceTracker, para todo terme acesso ao mesmo Dictionary
            services.AddSingleton<PresenceTracker>();
               
            return services;
        }
            
    }
}