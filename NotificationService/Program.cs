using MassTransit;
using NotificationService.MessageConsumers;
using NotificationService.Protos;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services
    .AddGrpcClient<PersonSearch.PersonSearchClient>(o =>
        o.Address = new Uri(builder.Configuration["GrpcUris:PersonsService"]));


builder.Services.AddMassTransit(opts =>
{
    opts.AddConsumer<AppointmentSetMessageConsumer>();
    opts.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("appointment_set_queue", e =>
        {
            e.ConfigureConsumer<AppointmentSetMessageConsumer>(context);
            e.UseConcurrencyLimit(1);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
