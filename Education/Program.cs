using Education.Services.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HttpClient
builder.Services.AddHttpClient();

// Read ApiConfig from configuration
var apiConfig = builder.Configuration.GetSection("ApiConfig").Get<ApiConfig>();
if (apiConfig == null)
{
    // Handle the case where ApiConfig is not present or is null
    throw new InvalidOperationException("ApiConfig is missing or null in appsettings.json.");
}

// Register ApiConfig with Dependency Injection
builder.Services.AddSingleton(apiConfig);

// Add session services
builder.Services.AddSession();

// Configure ApiService
builder.Services.AddScoped<IApiService, ApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Add session middleware
app.UseSession();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
