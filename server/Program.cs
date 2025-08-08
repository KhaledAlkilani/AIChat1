using AIChat1; 
using AIChat1.IService;
using AIChat1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AIChat1.Extensions;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];

// **VERIFY** by writing them out
Console.WriteLine($"[DEBUG] Using ConnectionString: {connectionString}");
Console.WriteLine($"[DEBUG] Using JwtSecretKey:    {jwtSecretKey}");

var allowFrontEndCors = "AllowFrontend";
var apiTitle = "AI Chat1";
var apiVersion = "v1";
var frontEndUrl = "http://localhost:5173";

// Setup Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repository & service registration
builder.Services.AddHttpClient<IChatService, ChatService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Enable Controllers & Endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// OpenAPI Configuration
builder.Services.AddSwaggerGen(options =>
{
    // Enable the annotations
    options.EnableAnnotations();

    options.SwaggerDoc(apiVersion, new OpenApiInfo
    {
        Title = apiTitle,
        Version = apiVersion,
        Description = "API documentation for managing game room bookings.",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@aichat1.com",
            Url = new Uri("https://aichat1app.com/support")
        }
    });
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowFrontEndCors, policy =>
    {
        policy.WithOrigins(frontEndUrl)
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add JSON options for enum serialization
// This allows enums to be serialized as strings in JSON responses
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("JWT secret key is not configured or is empty.");
}

var key = Encoding.UTF8.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Remove delay of token when expire
        };
    });

// Add SignalR and other necessary services
builder.Services.AddSignalR();
builder.Services.AddRazorPages();

// Add AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add authentication and authorization services
//builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(allowFrontEndCors);

// Enable OpenAPI UI. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", apiTitle);
    });
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable routing middleware
app.UseRouting();

// Enable CORS for the specified policy
app.UseCors("AllowAll");

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map the controllers
app.MapControllers();

// Map the SignalR hubs
app.MapSignalRHubs();

// Map Razor Pages
app.MapRazorPages();

app.Run();