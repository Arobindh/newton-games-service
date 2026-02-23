using Microsoft.EntityFrameworkCore;
using GamesService.Data;
using GamesService.Middleware;
using GamesService.Repositories;
using GamesService.Repositories.Interfaces;
using GamesService.Services;
using GamesService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Services
builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
