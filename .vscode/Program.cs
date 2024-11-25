using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração da autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"))
        };
    });

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/register-admin", async (Admin admin, AppDbContext context) =>
{
    context.Admins.Add(admin);
    await context.SaveChangesAsync();
    return Results.Created($"/admins/{admin.Id}", admin);
}).WithTags("Admin").RequireAuthorization();

app.MapPost("/register-vehicle", async (Vehicle vehicle, AppDbContext context) =>
{
    context.Vehicles.Add(vehicle);
    await context.SaveChangesAsync();
    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).WithTags("Vehicle").RequireAuthorization();

app.MapGet("/vehicles", async (AppDbContext context) =>
{
    var vehicles = await context.Vehicles.ToListAsync();
    return Results.Ok(vehicles);
}).WithTags("Vehicle");

app.Run();

// Modelo para Admin
public record Admin(int Id, string Username, string Password);

// Modelo para Vehicle
public record Vehicle(int Id, string Make, string Model, string PlateNumber);

// Contexto do Banco de Dados
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
}
