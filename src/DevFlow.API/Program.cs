using Microsoft.EntityFrameworkCore;
using DevFlow.Infrastructure.Data;
using DevFlow.Application.Interfaces;
using DevFlow.Infrastructure.Repositories;
using DevFlow.Application.Services; 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register DbContext
// Scoped = one instance per HTTP request
builder.Services.AddDbContext<DevFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
// Scoped = new instance for each HTTP request, disposed after request completes
// Why Scoped? DbContext is scoped, so repositories using it should be too
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();