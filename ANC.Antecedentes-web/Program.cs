using Microsoft.AspNetCore.Http.Features;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ANC.Modelo;

var builder = WebApplication.CreateBuilder(args);

// Agregamos configuraciones para las respuestas json.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(x =>
{
    x.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

});

builder.Services.AddControllersWithViews().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueCountLimit = int.MaxValue;
    x.MultipartBoundaryLengthLimit = int.MaxValue;
});
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromMinutes(140);
});

var configuration = builder.Configuration;
var ConnectionApiEndPint = configuration.GetConnectionString("WEBAPIERP_URL");
var ConnectionToken = configuration.GetConnectionString("WEBAPI_TOKEN");

var userAuth = builder.Configuration.GetValue<string>("ConnectionReniec:user_auth");
var passwordAuth = builder.Configuration.GetValue<string>("ConnectionReniec:password_auth");
var apiUrlAuth = builder.Configuration.GetValue<string>("ConnectionReniec:api_url_auth");
var apiUrlreniec = builder.Configuration.GetValue<string>("ConnectionReniec:api_url_reniec");

Env.SetConnectionStrings(ConnectionApiEndPint, ConnectionToken,userAuth,passwordAuth,apiUrlAuth,apiUrlreniec);
var ConnectionApiContext = new Env();
builder.Services.AddSingleton(ConnectionApiContext);

var secretKey = builder.Configuration.GetSection("JWT")["key"]; // Suponiendo que tengas una configuración en tu appsettings.json
builder.Services.AddSingleton<JwtManager>(new JwtManager(secretKey));


var app = builder.Build();

app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error"); // Agrega esta línea
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
