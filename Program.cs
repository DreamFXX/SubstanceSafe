var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()

builder.Services.AddDbContext<SubstanceSafeContext>(options => options.UseSqlite("Data Source=substance_usage.db"));

builder.Services.AddScoped();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
