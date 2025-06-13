using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Service;
using Backend.Repositories;
using Backend.Mappers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep property names as is
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Configure MySQL with EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
    
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Register services
builder.Services.AddScoped<ILessionService, LessionService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ILessionResultService, LessionResultService>();
builder.Services.AddScoped<NewDeepSeekService>();

// Register repositories
builder.Services.AddScoped<ILessionRepository, LessionRepository>();
builder.Services.AddScoped<ILessionImageRepository, LessionImageRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ISkillResultRepository, SkillResultRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();
builder.Services.AddScoped<ILessionAttemptRepository, LessionAttemptRepository>();
builder.Services.AddScoped<IAnswerAttemptRepository, AnswerAttemptRepository>();

var deepSeekApiKey = builder.Configuration["DeepSeekApiKey"]; 
var openRouterBaseUrl = builder.Configuration.GetValue<string>("OpenRouter:BaseUrl") ?? "https://openrouter.ai/api/v1";

if (string.IsNullOrEmpty(deepSeekApiKey))
{
    Console.WriteLine("CRITICAL WARNING: DeepSeekApiKey (OpenRouter API Key) is missing. AI Feedback service will likely fail.");
}
builder.Services.AddHttpClient<IDeepseekService, DeepseekService>(client =>
{
    client.BaseAddress = new Uri(openRouterBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(300);
    if (!string.IsNullOrEmpty(deepSeekApiKey))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deepSeekApiKey);
    }
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT key is missing in configuration (Jwt:Key).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });


// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure multipart form data limits (equivalent to max-file-size: 10MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

app.UseCors("AllowFrontend");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();