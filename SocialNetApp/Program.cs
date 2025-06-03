using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetApp;
using SocialNetApp.Data;
using SocialNetApp.Data.Models;
using SocialNetApp.Data.Repository;
using SocialNetApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"), b => b.MigrationsAssembly("SocialNetApp")))
    .AddUnitOfWork()
    .AddCustomRepository<Message, MessageRepository>()
    .AddCustomRepository<Friend, FriendsRepository>();
builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 6;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var mapperConfig = new MapperConfiguration(v =>
{
    v.AddProfile(new MappingProfile());
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
var cachePeriod = "0";
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
    }
});

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
    if (await userManager.FindByNameAsync("Admin") == null)
    {
        var user = new User
        {
            UserName = "Admin",
            FirstName = "Admin",
            LastName = "Admin",
            MiddleName = "Admin",
            Email = "admin@example.com",
            BirthDate = DateTime.Parse("01.01.2000"),
            Image = "https://avatar.iran.liara.run/public/boy",
            Status = "Admin",
            About = "Admin"
        };
        await userManager.CreateAsync(user, "1234567");
        await userManager.AddToRoleAsync(user, "Admin");
    }
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
