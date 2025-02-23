using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Common.SQL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SlqStudio.Application.ApiClients.Moodle;
using SlqStudio.Application.ApiClients.Moodle.Models;
using SlqStudio.Application.CQRS.Course.Commands;
using SlqStudio.Application.CQRS.Course.Commands.Handlers;
using SlqStudio.Application.CQRS.Course.Queries;
using SlqStudio.Application.CQRS.Course.Queries.Handlers;
using SlqStudio.Application.CQRS.LabTask.Commands;
using SlqStudio.Application.CQRS.LabTask.Commands.Handler;
using SlqStudio.Application.CQRS.LabTask.Queries;
using SlqStudio.Application.CQRS.LabTask.Queries.Handler;
using SlqStudio.Application.CQRS.LabWork.Commands;
using SlqStudio.Application.CQRS.LabWork.Commands.Handlers;
using SlqStudio.Application.CQRS.LabWork.Queries;
using SlqStudio.Application.CQRS.LabWork.Queries.Handlers;
using SlqStudio.Application.Services;
using SlqStudio.Application.Services.Implementation;
using SlqStudio.Application.Services.Models;
using SlqStudio.Persistence;
using SlqStudio.Persistence.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString =builder.Configuration.GetConnectionString("MySQL");
var labsConnectionString = builder.Configuration.GetConnectionString("LabsConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddSingleton<SqlManager>(sp => new SqlManager(labsConnectionString));


builder.Services.AddMediatR(
    typeof(CreateCourseCommandHandler).Assembly,
    typeof(UpdateCourseCommandHandler).Assembly,
    typeof(DeleteCourseCommandHandler).Assembly,
    typeof(GetAllCoursesQueryHandler).Assembly,
    typeof(GetCourseByIdQueryHandler).Assembly,
    typeof(CreateLabWorkCommandHandler).Assembly,
    typeof(UpdateLabWorkCommandHandler).Assembly,
    typeof(DeleteLabWorkCommandHandler).Assembly,
    typeof(GetAllLabWorksQueryHandler).Assembly,
    typeof(GetLabWorkByIdQueryHandler).Assembly,
    typeof(CreateTaskCommandHandler).Assembly,
    typeof(UpdateTaskCommandHandler).Assembly,
    typeof(DeleteTaskCommandHandler).Assembly,
    typeof(GetAllTasksQueryHandler).Assembly,
    typeof(GetTaskByIdQueryHandler).Assembly);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? string.Empty))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["jwt"];
                return Task.CompletedTask;
            }
        };
    });

var jwtSettings = new JwtSettings
{
    Key = builder.Configuration["Jwt:SecretKey"],
    Issuer = builder.Configuration["Jwt:Issuer"],
    Audience = builder.Configuration["Jwt:Audience"],
    ExpirationMinutes = 30
};
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddHttpClient();

builder.Services.Configure<MoodleApiSettings>(builder.Configuration.GetSection("MoodleApi"));
builder.Services.AddSingleton<MoodleApiClient>(); 

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
builder.Services.AddScoped<IMoodleService, MoodleService>();
;
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

app.Run();