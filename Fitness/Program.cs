using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Fitness.Data;
using Fitness.Models;
using Fitness.Pages;
using Fitness.Areas.Identity.Pages;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var connectionString2 = builder.Configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

builder.Services.AddDbContext<FitnessChallengeContext>(options =>
    options.UseSqlServer(connectionString2));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

/*app.Use(async (context, next) =>
{
    var user = context.User;
    var path = context.Request.Path;
    if (!user.Identity.IsAuthenticated && 
        path != "/Identity/Account/Login" && 
        path != "/Identity/Account/Register" && 
        path != "/Identity/Account/Logout" && 
        !path.StartsWithSegments("/Identity/Account/Manage"))
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }
    await next();
});*/
app.MapRazorPages();

app.Run();
