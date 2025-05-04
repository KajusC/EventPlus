using eventplus.models.Domain;
using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.Persistance.Repositories;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.Handlers;
using EventPlus.Server.Application.IHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace EventPlus.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:5173")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
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

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Logic
            builder.Services.AddScoped<IEventLogic, EventLogic>();
            builder.Services.AddScoped<ITicketLogic, TicketLogic>();
            builder.Services.AddScoped<IFeedbackLogic, FeedbackLogic>();
            builder.Services.AddScoped<ISectorLogic, SectorLogic>();


            // AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

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
            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}

