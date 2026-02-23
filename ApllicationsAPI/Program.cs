using System.Reflection;
using ApllicationsAPI.Commands;
using ApllicationsAPI.Models.Data;
using ApplicationsAPI.Protos;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddGrpcClient<DocumentSearch.DocumentSearchClient>(o =>
        o.Address = new Uri(builder.Configuration["GrpcUris:DocumentsService"]));
builder.Services
    .AddGrpcClient<PersonSearch.PersonSearchClient>(o =>
        o.Address = new Uri(builder.Configuration["GrpcUris:PersonsService"]));

builder.Services
    .AddMassTransit(opts => opts.UsingRabbitMq());

builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (NotFoundException)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new
        {
            message = "Resource not found",
            traceId = context.TraceIdentifier
        });
    }
    catch (BadRequestException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new
        {
            message = ex.Message,
            traceId = context.TraceIdentifier
        });
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Applications API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at /
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();