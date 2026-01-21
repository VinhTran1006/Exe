using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Implementations;
using Agriculture_Analyst.Repositories.Interfaces;
using Agriculture_Analyst.Services.Implementations;
using Agriculture_Analyst.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});

// Load environment variables
builder.Configuration.AddEnvironmentVariables();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AgricultureAnalystDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDiaryRepository, DiaryRepository>();
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<IPlantTaskRepository, PlantTaskRepository>();

// Register Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDiaryService, DiaryService>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IPlantTaskService, PlantTaskService>();
builder.Services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();

// Add Authentication and Cookie Middleware
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/SignIn"; // Redirect to SignIn page if not authenticated
        options.AccessDeniedPath = "/Home/AccessDenied"; // Redirect if access is denied
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expiration time
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use Session BEFORE Authorization
app.UseSession();

// Add Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
