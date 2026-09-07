using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// Add MVC
// =========================================================
builder.Services.AddControllersWithViews();

// =========================================================
// Connect to Oracle Database
// =========================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(
        builder.Configuration.GetConnectionString("OracleConnection")
    )
);

// =========================================================
// Session
// =========================================================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =========================================================
// Build application
// =========================================================
var app = builder.Build();

// =========================================================
// HTTP request pipeline
// =========================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// =========================================================
// Default MVC Route
// =========================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();