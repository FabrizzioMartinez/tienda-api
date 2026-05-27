using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Tienda.API.Data;
using Tienda.API.Interfaces;
using Tienda.API.Interfaces.Cliente;
using Tienda.API.Interfaces.Venta;
using Tienda.API.Models;
using Tienda.API.Services;
using Tienda.API.Services.Cliente;
using Tienda.API.Services.Venta;
using Npgsql;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos
var connectionString = builder.Configuration.GetConnectionString("TiendaConnection");
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Registro de Servicios
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<ITipoProductoService, TipoProductoService>();
builder.Services.AddScoped<IUnidadMedidaService, UnidadMedidaService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IMaestroService, MaestroService>();

// 3. Configuración de CORS - Agregamos todas las variantes posibles
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200", 
            "https://tienda-front-khaki.vercel.app", // Tu dominio actual
            "https://tienda-front.vercel.app",       // Posible dominio principal
            "https://tienda-front-5d8p9j12g-fabrizziomartinezs-projects.vercel.app" // Tu URL larga
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// 4. Configuración de Controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

var app = builder.Build();

// 5. Middlewares
app.UseCors("PermitirAngular"); // <-- Crucial: debe estar antes de MapControllers
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
