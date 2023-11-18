using Microsoft.EntityFrameworkCore;
using Udemy.DataAccess.Data;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Udemy.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// добавление роли в Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

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

app.UseRouting();
app.UseAuthentication(); // Необходимо добавить UseAuthentication() перед UseAuthorization(), изначально происходит проверка действительности имени пользователя или пароля
app.UseAuthorization();// Если имя пользователя и пароль действительны, авторизация происходит
app.MapRazorPages(); // добавление маршрутизации которая нужна для маппирования Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
