using CharacterManagerApiTutorial.Data;
using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:4200") // Angular dev server URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // If you use cookies or auth headers
    );
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register the database context with the dependency injection container using SQL Server (pulled from appsettings.json).
builder.Services.AddDbContext<CharacterManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configures JWT Bearer authentication for the application. This setup ensures only valid, signed JWTs from trusted sources are accepted.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validates the issuer, audience, token lifetime, and signing key.
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
            ValidateIssuerSigningKey = true
        };
    });

// Register application services with scoped lifetime for dependency injection (AddScoped means a new instance is created per HTTP request).
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICharacterMetadataService, CharacterMetadataService>();
builder.Services.AddScoped<ILookupDataService, LookupDataService>();

// Add Swagger services to the container
builder.Services.AddEndpointsApiExplorer();  // Adds support for discovering and documenting endpoints so they can be shown in Swagger UI.
builder.Services.AddSwaggerGen(options =>
{
    // Define the Swagger document metadata for version "v1". This sets the title, version, and description that will appear in the Swagger UI
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Character Manager API Tutorial",
        Version = "v1",
        Description = "An API tutorial for managing characters"
    });

    // Add JWT bearer auth scheme to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    // Add requirement to use the security scheme globally
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
});

// Configure rate limiting services using a fixed window algorithm
builder.Services.AddRateLimiter(options =>
{
    // Add a policy named "Fixed" that applies rate limiting based on the client's IP address
    options.AddPolicy("Fixed", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),  // Use client IP as partition key
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,                // Allow a maximum of 5 requests...
                Window = TimeSpan.FromMinutes(1), // ...per 1 minute
                QueueLimit = 0,                 // Do not queue additional requests once the limit is reached
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst  // (Irrelevant here since QueueLimit is 0)
            }));
});

var app = builder.Build();

// Use CORS policy before routing and endpoints
app.UseCors("AllowFrontend");

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Character Manager API Tutorial v1");
        options.RoutePrefix = ""; // sets Swagger UI at the app root (optional)
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
