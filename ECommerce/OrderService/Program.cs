using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderService;
using OrderService.MQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Ecom_OrderDBContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("OrderDBDatabase")));
builder.Services.AddScoped<IRabitMQProducer, RabitMQProducer>();

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
