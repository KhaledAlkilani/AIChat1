using AIChat1; 
using AIChat1.Extensions;
using AIChat1.IService;
using AIChat1.Options;
using AIChat1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

DotNetEnv.Env.Load(); // 1) load .env early
var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Setup Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];

// OpenAI
var openAiKey = builder.Configuration["OpenAI:ApiKey"];
var openAiModel = builder.Configuration["OpenAI:Model"] ?? "gpt-4o-mini";
// Repository & service registration
// Bind OpenAI options from configuration (.env -> configuration)
builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddScoped<IChatService, ChatService>();
// LLM client: typed HttpClient + your implementation
builder.Services.AddHttpClient<ILLMClient, OpenAiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/"); // stable base
    // you can set default headers here OR in OpenAiClient per-request
});

// 2) bind JwtSettings once, register issuer
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtIssuer, JwtIssuer>();

string Mask(string? s) => string.IsNullOrEmpty(s) ? "<empty>" :
    (s.Length <= 8 ? "****" : $"{s[..4]}****{s[^4..]}");
Console.WriteLine($"[DEBUG] OpenAI ApiKey:    {Mask(openAiKey)}");
Console.WriteLine($"[DEBUG] OpenAI Model:     {openAiModel}");

// **VERIFY** by writing them out
Console.WriteLine($"[DEBUG] Using ConnectionString: {connectionString}");
Console.WriteLine($"[DEBUG] Using JwtSecretKey:    {jwtSecretKey}");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Controllers / Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();


var allowFrontEndCors = "AllowFrontend";
var apiTitle = "AI Chat1";
var apiVersion = "v1";
var frontEndUrl = "http://localhost:5173";

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
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
    });
});

if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("JWT secret key is not configured or is empty.");
}

// 3) auth: read JwtSettings and configure bearer validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        opts.RequireHttpsMetadata = false; // true in prod
        opts.SaveToken = true;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // If the request is for our SignalR hub
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Add SignalR and other necessary services
builder.Services.AddSignalR();
// Add custom user ID provider for SignalR
builder.Services.AddSingleton<IUserIdProvider, SubClaimUserIdProvider>();
builder.Services.AddRazorPages();

// Add AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add authentication and authorization services
//builder.Services.AddAuthorization();

var app = builder.Build();  

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
app.UseCors(allowFrontEndCors);

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