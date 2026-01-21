using MediatR;
using Mapster;
using Swashbuckle.AspNetCore.SwaggerGen;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.RegisterMapsterConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
