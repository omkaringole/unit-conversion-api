using UnitConverter.Api.Middleware;
using UnitConverter.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Catalog is stateless and cheap to build, but registering it as a
// singleton avoids rebuilding the unit dictionary on every request.
builder.Services.AddSingleton<IUnitCatalog, UnitCatalog>();
builder.Services.AddScoped<IConversionService, ConversionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Unit Conversion API",
        Version = "v1",
        Description = "Converts numerical values between units of length, mass and temperature."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ConversionExceptionMiddleware>();

app.MapControllers();

app.Run();

// Exposed so the test project can spin up the app via WebApplicationFactory.
public partial class Program
{
}
