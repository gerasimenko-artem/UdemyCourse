using Microsoft.EntityFrameworkCore;
using Udemy.DataAccess.Data;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Udemy.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// ���������� ���� � Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

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
    options.ClientId = "239948002941-g0a7pvg7f72c464o73t4895qgslv2pnj.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages(); // � ��������� ���� ������ ��������� MVC ��������� � ������ ���������� Razor Pages ���������� �������� ������ ������������
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
app.UseAuthentication(); // ���������� �������� UseAuthentication() ����� UseAuthorization(), ���������� ���������� �������� ���������������� ����� ������������ ��� ������
app.UseSession();
app.UseAuthorization();// ���� ��� ������������ � ������ �������������, ����������� ����������
app.MapRazorPages(); // ���������� ������������� ������� ����� ��� ������������ Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
