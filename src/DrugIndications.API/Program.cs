using DrugIndications.Domain.Interfaces;
using DrugIndications.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using DrugIndications.Application.Services;
using DrugIndications.Infrastructure.Services;
using DrugIndications.Application.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DrugIndications.API.Seed;



var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:80");
builder.Services.Configure<OpenAIOptions>(
    builder.Configuration.GetSection("OpenAI"));

builder.Services.AddScoped<IEligibilityParser, EligibilityParser>();
builder.Services.AddSingleton<EligibilityParserService>();
builder.Services.AddSingleton<IndicationExtractorService>();

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();



// Swagger y Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Drug Indications API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'token'. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    options.EnableAnnotations();
});

// Obtener cadena de conexi�n desde env vars (Docker) o appsettings
var connectionString = builder.Configuration.GetConnectionString("Default")

    ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");

// Inyectar el repositorio
builder.Services.AddSingleton<IProgramRepository>(new ProgramRepository(connectionString!));

var app = builder.Build();

// Activar Swagger si es entorno de desarrollo
// Swagger middleware
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Registrar controllers
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var repo = app.Services.GetRequiredService<IProgramRepository>();

app.Run();
