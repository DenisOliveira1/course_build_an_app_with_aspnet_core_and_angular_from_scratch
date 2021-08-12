using System.Text;
using System.Threading.Tasks;
using api.Context;
using api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace api.Extentions
{
    public static class IdentityServiceExtensions
    {
         public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config){

            services.AddIdentityCore<UserModel>(opt => {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase= false;
                opt.Password.RequireDigit = false;
            })
                .AddRoles<RoleModel>()
                .AddRoleManager<RoleManager<RoleModel>>()
                .AddSignInManager<SignInManager<UserModel>>()
                .AddRoleValidator<RoleValidator<RoleModel>>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        ValidateIssuer = false, // api
                        ValidateAudience = false // client
                    };

                    options.Events = new JwtBearerEvents{
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")){
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(opt => {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin","Moderator"));
            });

            return services;
        }
    }
}