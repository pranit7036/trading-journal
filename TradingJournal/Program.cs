using EvolveDb;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TradingJournal.Context;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Interfaces.Services;
using TradingJournal.Repository;
using TradingJournal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<ITradeService, TradesService>();
builder.Services.AddScoped<ITradeRepository, TradesRepository>();

var app = builder.Build();


// Evolve Configuration
using (var connection = new NpgsqlConnection(
    builder.Configuration.GetConnectionString("DefaultConnection")))
{
    var evolve = new Evolve(connection)
    {
        Locations = new[] { "Database/Migrations" },
        IsEraseDisabled = true
    };

    evolve.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
