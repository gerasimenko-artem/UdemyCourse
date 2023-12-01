using Microsoft.EntityFrameworkCore;
using Udemy.DataAccess.Data;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Udemy.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Udemy.DataAccess.DbInitializer;
using Udemy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// добавление роли в Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();




builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication().AddFacebook(options =>
{
	options.AppId = "1091743118413076";
	options.AppSecret = "1f0ab196162c3a9ee934363939f654b1";
});

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "239948002941-kjtshgb5jl4vaptllo58sem3bpfppf9l.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages(); // в программе есть только поддержка MVC поскольку в проэкт добавились Razor Pages необходимо добавить данную конфигурацию
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication(); // Необходимо добавить UseAuthentication() перед UseAuthorization(), изначально происходит проверка действительности имени пользователя или пароля
app.UseSession();
SeedDatabase();
app.UseAuthorization();// Если имя пользователя и пароль действительны, авторизация происходит
app.MapRazorPages(); // добавление маршрутизации которая нужна для маппирования Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}
}
