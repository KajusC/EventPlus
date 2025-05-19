using eventplus.models.Domain;
using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.Persistance.Repositories;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.Authentication;
using EventPlus.Server.Application.Handlers;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventPlus.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddHostedService<BackgroundPricingService>();
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventPlus API", Version = "v1" });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:5173")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:5173")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });
            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Temporarily disable for debugging
                    ValidateAudience = false, // Temporarily disable for debugging
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "DefaultSecretKeyWith32Characters1234")),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };
            });

            builder.Services.AddDbContext<EventPlusContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<ISectorRepository, SectorRepository>();
            builder.Services.AddScoped<IRepository<EventLocation>, EventLocationRepository>();
            builder.Services.AddScoped<IRepository<Partner>, PartnerRepository>();
            builder.Services.AddScoped<IRepository<Performer>, PerformerRepository>();
            builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<ISectorPriceRepository, SectorPriceRepository>();
			builder.Services.AddScoped<ISeatingRepository, SeatingRepository>();
            builder.Services.AddScoped<IUserRequestAnswerRepository, UserRequestAnswerRepository>();

			// Register the new repositories for role-based authentication
			builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
            builder.Services.AddScoped<IOrganiserRepository, OrganiserRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Logic
            builder.Services.AddScoped<IEventLogic, EventLogic>();
            builder.Services.AddScoped<ITicketLogic, TicketLogic>();
            builder.Services.AddScoped<IFeedbackLogic, FeedbackLogic>();
            builder.Services.AddScoped<ISectorLogic, SectorLogic>();
            builder.Services.AddScoped<ICategoryLogic, CategoryLogic>();
            builder.Services.AddScoped<ISectorPriceLogic, SectorPriceLogic>();
            builder.Services.AddScoped<ISeatingLogic, SeatingLogic>();
            builder.Services.AddScoped<IOrganiserLogic, OrganiserLogic>();
            builder.Services.AddScoped<IUserRequestAnswerLogic, UserRequestAnswerLogic>();
            builder.Services.AddScoped<IQuestionLogic, QuestionLogic>();

            // Authentication - Use role-based auth service
            builder.Services.AddScoped<IRoleBasedAuthService, RoleBasedAuthService>();

            // AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add background services
            builder.Services.AddHostedService<TicketInvalidationService>();

            var app = builder.Build();
            app.UseCors("AllowFrontend");
            if (builder.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            // Add authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}

