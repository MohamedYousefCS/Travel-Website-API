using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.DTO.PaymentClasses;
using Travel_Website_System_API_.Hubs;
using Travel_Website_System_API_.Repositories;
using Travel_Website_System_API_.UnitWork;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;

namespace Travel_Website_System_API_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            Configure(app);

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add controllers
            services.AddControllers();

            // Add unit of work
            services.AddScoped<UnitOFWork>();

            // JSON options
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });

            // Add Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Configure DbContext
            services.AddDbContext<ApplicationDBContext>(op => op.UseSqlServer(configuration.GetConnectionString("Connection")));

            // Configure PayPal settings
            services.Configure<PayPalSettings>(configuration.GetSection("PayPal"));

            // Configure Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

            // Repositories and other services
            services.AddScoped<UserRepo>();
            services.AddScoped<IGenericRepo<Client>, GenericRepo<Client>>();
            services.AddScoped<IGenericRepo<Admin>, GenericRepo<Admin>>();
            services.AddScoped<IGenericRepo<CustomerService>, GenericRepo<CustomerService>>();
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddScoped<GenericRepository<Service>>();
            services.AddScoped<GenericRepository<Package>>();
            services.AddScoped<GenericRepository<ServiceProvider>>();
            services.AddScoped<GenericRepository<LoveService>>();
            services.AddScoped<GenericRepository<LovePackage>>();
            services.AddScoped<GenericRepository<Category>>();

            // JWT Authentication configuration
            ConfigureAuthentication(services, configuration);

            // Add SignalR
            services.AddSignalR();
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:ValidIssuer"],
                    ValidAudience = configuration["Jwt:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            })
            .AddGoogle(options =>
            {
                var googleAuthNSection = configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
            })
            .AddFacebook(options =>
            {
                var fbAuthNSection = configuration.GetSection("Authentication:Facebook");
                options.AppId = fbAuthNSection["AppId"];
                options.AppSecret = fbAuthNSection["AppSecret"];
            })
            .AddTwitter(options =>
            {
                var twitterAuthNSection = configuration.GetSection("Authentication:Twitter");
                options.ConsumerKey = twitterAuthNSection["ConsumerKey"];
                options.ConsumerSecret = twitterAuthNSection["ConsumerSecret"];
            });
        }

        public static void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            SeedRoles(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // CORS policy
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/ChatHub", options =>
                {
                    options.Transports =
                        HttpTransportType.WebSockets |
                        HttpTransportType.LongPolling;
                });
            });
            app.Run();
        }

        private static void SeedRoles(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var seed = new DataSeed(roleManager);
                seed.SeedRolesAsync().GetAwaiter().GetResult();
            }
        }

    }
}
