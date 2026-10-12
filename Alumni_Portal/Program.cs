using Alumni_Portal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "");
// MVC
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddScoped<Alumni_Portal.Services.EmailService>();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

// Localization ayarları
var supportedCultures = new[] { "tr-TR", "en-US", "de-DE" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("tr-TR")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);

    options.FallBackToParentCultures = false;
    options.FallBackToParentUICultures = false;

    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
});

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookie Authentication
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath        = "/Login/Index";
        options.LogoutPath       = "/Login/Logout";
        options.AccessDeniedPath = "/Login/AccessDenied";
        options.ExpireTimeSpan   = TimeSpan.FromHours(8);
    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<Alumni_Portal.Hubs.ChatHub>("/chatHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();