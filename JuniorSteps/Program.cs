using JuniorSteps.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSession();
builder.Services.AddScoped<SimpleAuthFilter>();
builder.Services.AddRazorPages(); // ✅ Razor Pages servisi

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/all-posts");
        return;
    }

    await next();
});

app.UseRouting();
app.UseSession();
app.UseAuthorization();

// ✅ Razor Pages route'ları
app.MapRazorPages();

// MVC route'ları
app.MapControllerRoute(
    name: "customPosts",
    pattern: "all-posts",
    defaults: new { controller = "Post", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}");

app.Run();
