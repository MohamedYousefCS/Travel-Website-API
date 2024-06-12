
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.UnitWork;

namespace Travel_Website_System_API_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // inject unit of work
            builder.Services.AddScoped<UnitOFWork>();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddSwaggerGen(op=>
            //op.SwaggerDoc("Version 1",new Microsoft.OpenApi.Models.OpenApiInfo()
            //{
            //                    Version = "v2",
            //                     Title = "Web API Travel Website",
            //                     Description = "This is a web api for Travel Website"

            //})
            //);

            builder.Services.AddDbContext<ApplicationDBContext>(op=>op.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
             .AddEntityFrameworkStores<ApplicationDBContext>()
             .AddDefaultTokenProviders();



            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Add CORS policy
            app.UseCors(policy =>

            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

    }
            
    }

