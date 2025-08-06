using AIChat1;
using server.Hubs;
using AIChat1.IService;
using AIChat1.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var allowFrontEndCors = "AllowFrontend";
var apiTitle = "Game Room Booking API";
var apiVersion = "v1";
var frontEndUrl = "http://localhost:5173";

// Setup Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add SignalR and other necessary services
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// 1) Make sure routing comes first
app.UseRouting();

// 2) Then plug in CORS _before_ any hubs or controllers
app.UseCors("AllowAll");

// (If you have authentication/authorization, it goes here)
// app.UseAuthentication();
// app.UseAuthorization();

// 3) Now map your real‐time and HTTP endpoints
app.MapHub<ChatHub>("/chat");
app.MapControllers();
app.MapRazorPages();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();