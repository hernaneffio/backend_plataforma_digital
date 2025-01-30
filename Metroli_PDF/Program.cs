using Metroli_PDF;
using Microsoft.OpenApi.Models;
using Metroli_PDF.Middleware;
using Org.BouncyCastle.Security;
using Infraestructure;
using crypto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen((options) =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
       {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
       }
    });
});

var app = builder.Build();


app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Backend Metroli PDF - {app.Environment.EnvironmentName}");
    c.InjectStylesheet("css/SwaggerDark.css"); // Ruta al archivo CSS personalizado
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseCors(x => x
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true)
                  .AllowCredentials());

app.UseAuthentication();
app.UseMiddleware<ValidacionTokenPersonalizadaMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
