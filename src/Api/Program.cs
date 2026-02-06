using System.Text;
using Api.Auth;
using Infrastructure;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the raw JWT here (no 'Bearer ' prefix). Swagger UI will send it as: Authorization: Bearer {token}"
    };

    options.AddSecurityDefinition("Bearer", scheme);

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", hostDocument: document, externalResource: null), new List<string>() }
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Missing connection string 'ConnectionStrings:Default'. " +
        "Set it via environment variable ConnectionStrings__Default or dotnet user-secrets for src/Api/Api.csproj.");
}

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var jwtKey = builder.Configuration["Jwt:Key"]; // validated by JwtTokenService too
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Missing JWT signing key 'Jwt:Key'. Set it via environment variable Jwt__Key or dotnet user-secrets for src/Api/Api.csproj.");
}

var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "JWT signing key 'Jwt:Key' is too short for HS256. Use at least 32 bytes (e.g. 32+ random characters). " +
        "The setup scripts generate a suitable dev key automatically.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Winecellar";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Winecellar";
var signingKey = new SymmetricSecurityKey(jwtKeyBytes);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = builder.Environment.IsDevelopment();

        if (builder.Environment.IsDevelopment())
        {
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    // Default challenge gives very little information when the header is missing.
                    // In Development, return a small JSON payload to make debugging Swagger/auth easier.
                    var authHeader = context.Request.Headers.Authorization.ToString();
                    var hasBearer = !string.IsNullOrWhiteSpace(authHeader);

                    var error = context.Error;
                    var errorDescription = context.ErrorDescription;

                    if (!hasBearer)
                    {
                        error ??= "missing_authorization";
                        errorDescription ??= "No Authorization header was sent. In Swagger, click Authorize and paste the raw JWT (no 'Bearer ' prefix).";
                    }

                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var payload = new
                    {
                        error,
                        error_description = errorDescription
                    };

                    return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                }
            };
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
