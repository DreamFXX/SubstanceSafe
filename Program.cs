using Microsoft.EntityFrameworkCore;
using SubstanceSafe.Services; // Assuming SubstanceSafeContext is in this namespace
using SubstanceSafe.Models; // Assuming Substance is in this namespace
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SubstancesDbContext>(options => // Changed SubstanceSafeContext to SubstancesDbContext
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
