using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(o => o.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.IdleTimeout = TimeSpan.FromHours(2); o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; });
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("Backend", c => c.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:7001/"));
builder.Services.AddScoped<ApiClient>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseSession(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
