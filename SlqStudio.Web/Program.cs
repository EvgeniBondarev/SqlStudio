using SlqStudio.Extensions;
using SlqStudio.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ConfigureLogging(builder.Configuration);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddCustomMediatR();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddMoodleServices(builder.Configuration);
builder.Services.AddJwtTokenServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCustomStatusCodePages();
app.UseErrorHandling();

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

app.Run();