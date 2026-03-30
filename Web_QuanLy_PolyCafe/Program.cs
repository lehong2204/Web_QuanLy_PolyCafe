
////app.Run();
//using Microsoft.EntityFrameworkCore;
//using Web_QuanLy_PolyCafe.Data;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//// Đăng ký DbContext - kết nối SQL Server localhost
//builder.Services.AddDbContext<PolyCafeDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("PolyCafeDb")));

//// ✅ Đăng ký Session
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromHours(8); // Session tồn tại 8 giờ
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseRouting();

//// ✅ Bật Session (phải đặt TRƯỚC UseAuthorization)
//app.UseSession();

//app.UseAuthorization();

//app.MapStaticAssets();

//// ✅ THÊM route Area vào TRƯỚC default
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

//// Route mặc định
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{id?}")
//    .WithStaticAssets();

//app.Run();
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;
using Web_QuanLy_PolyCafe.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PolyCafeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PolyCafeDb")));

// ✅ Đăng ký EmailService
builder.Services.AddSingleton<EmailService>();

// ✅ Đăng ký Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();