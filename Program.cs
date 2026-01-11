using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using newltweb.Models;
using newltweb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext <LtwebBtlContext> (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<TUser, IdentityRole>()
    .AddEntityFrameworkStores<LtwebBtlContext>()
    .AddDefaultTokenProviders();
// Authentication / Authorization (example cookie)
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Account/Login";
    opts.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddScoped<ICartService, EfCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IUserIdProvider, UserIdProvider>();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//switch to https, enable it later
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

//so ASP.NET Core can read and validate an authentication cookie
app.UseAuthentication();

app.UseAuthorization();

// Area route (must be registered before default)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
