using Application.Commands;
using Application.Queries;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repository;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repos (Vertical Slice)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Warehouse CQRS API", Version = "v1" }));
//builder.Services.RegisterMapsterConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse CQRS API"));
}

app.MapPost("/orders", async (CreateOrderCommand cmd, IMediator mediator) =>
    await mediator.Send(cmd))
    .Produces<Guid>()
    .WithName("CreateOrder")
    .WithTags("Orders")
    .WithSummary("Create new warehouse order");


//{
//    "customerId": "7efc0b43-8c31-4fcb-9d2f-54f0d80c8f23",
//  "items": [
//    { "productId": "3c4c2d9e-...", "price": 10.0, "quantity": 2 }
//  ]
//}


app.MapGet("/orders/{id}", async (Guid id, IMediator mediator) =>
    await mediator.Send(new GetOrderByIdQuery(id)))
    .Produces<Application.Queries.OrderDto>()
    .WithName("GetOrder")
    .WithTags("Orders");

app.Run();
