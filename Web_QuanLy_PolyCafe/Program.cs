//using Microsoft.EntityFrameworkCore;
//using Web_QuanLy_PolyCafe.Data;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//// ??ng ký DbContext - k?t n?i SQL Server localhost
//builder.Services.AddDbContext<PolyCafeDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("PolyCafeDb")));

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseRouting();

//app.UseAuthorization();

//app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Order}/{action=POS}/{id?}")
//    .WithStaticAssets();


//app.Run();
using Microsoft.EntityFrameworkCore;
using Web_QuanLy_PolyCafe.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext - kết nối SQL Server localhost
builder.Services.AddDbContext<PolyCafeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PolyCafeDb")));

// ✅ Đăng ký Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8); // Session tồn tại 8 giờ
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ✅ Bật Session (phải đặt TRƯỚC UseAuthorization)
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// ✅ Đổi default route về Login thay vì POS
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();