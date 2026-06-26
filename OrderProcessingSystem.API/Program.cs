using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Interfaces;
using OrderProcessingSystem.Application.Services;
using OrderProcessingSystem.Infrastructure.Data;
using OrderProcessingSystem.Infrastructure.Messaging;
using OrderProcessingSystem.Infrastructure.Repositories;
using OrderProcessingSystem.Infrastructure.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();

builder.Services.AddHostedService<OrderProcessingWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order Processing API", Version = "v1" });
});

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Processing API v1");
    c.RoutePrefix = string.Empty; // Swagger UI at root: http://localhost:5000/
});

app.MapControllers();

app.Run();
