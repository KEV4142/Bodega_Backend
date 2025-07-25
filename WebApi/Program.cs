using System.Globalization;
using Aplicacion;
using DotNetEnv;
using Persistencia;
using WebApi.Extensions;
using WebApi.Middleware;

Env.Load();

var basePath = AppContext.BaseDirectory;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = basePath,
    Args = args
});
builder.Configuration
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "ConnectionStrings:DefaultConnection", Environment.GetEnvironmentVariable("DB_CONNECTION")! },
        { "TokenKey", Environment.GetEnvironmentVariable("TOKEN_KEY")! },
        { "AllowedHosts", Environment.GetEnvironmentVariable("BACKEND_ORIGIN")! },
        { "AllowedFrontendHosts", Environment.GetEnvironmentVariable("FRONTEND_ORIGIN")! }
    });

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddApplicacion();
builder.Services.AddPersistencia(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddPoliciesServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();

var allowedFrontendOrigins = builder.Configuration.GetSection("AllowedFrontendHosts").Get<string>();
builder.Services.AddCors(o => o.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins(allowedFrontendOrigins!).AllowAnyMethod().AllowAnyHeader();
}));

var port = Environment.GetEnvironmentVariable("PORT"); 
if (string.IsNullOrEmpty(port))
{
    port = "5000";
}
Console.WriteLine($"Servidor iniciando en el puerto: {port}");
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddOutputCache();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.useSwaggerDocumentation();
}

app.UseAuthentication();
app.UseAuthorization();

//await app.SeedDataAuthentication();
app.UseCors("corsapp");
app.MapControllers();

app.UseOutputCache();
app.Run();