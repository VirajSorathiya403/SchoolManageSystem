var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Enable Session Management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Cookie settings for security
    options.Cookie.IsEssential = true; // Ensure the cookie is always available
});

// Configure the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Enable serving static files (CSS, JS, images, etc.)
app.UseStaticFiles();

// Enable session middleware
app.UseSession();

// Enable routing
app.UseRouting();

// Authentication/Authorization middleware (if needed)
app.UseAuthorization();

// Define default route mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Admin}/{action=Index}/{id?}");

// Start the app
app.Run();