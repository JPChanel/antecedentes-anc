using Microsoft.IdentityModel.Tokens;
using ANC.WebApi.Services;
using System.Text;
using ANC.Conexion;

System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//inicializar configuraciones de conexion BD
var configuration = builder.Configuration;
var ConnectionSybaseUsis = configuration.GetConnectionString("LINQ_SYBASE_USIS");
var ConnectionSybaseZav = configuration.GetConnectionString("LINQ_SYBASE_ZAV");
var ConnectionPG = configuration.GetConnectionString("LINQ_PG");
ConexionBD.SetConnectionStrings(ConnectionSybaseUsis, ConnectionSybaseZav, ConnectionPG);
var dbContext = new ConexionBD();
builder.Services.AddSingleton(dbContext);


// Registra la clase TwilioService como un servicio singleton
var secretKey = builder.Configuration.GetSection("JWT")["key"]; // Suponiendo que tengas una configuración en tu appsettings.json
builder.Services.AddSingleton<JwtManager>(new JwtManager(secretKey));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseHsts();
}



app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
