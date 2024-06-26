using COTSEClient.Helper;
using COTSEClient.Hubs;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<Sep490G17DbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection"));
});

builder.Services.AddSignalR();
builder.Services.AddCors();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Common/AccessDenied";
    });

// Add scope repository
AddServiceCollectionDI.AddScopeServiceCollectionDI(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Add options Cors
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapFallbackToPage("/Common/NotFound");

app.MapHub<ParticiPantScoresHub>("/particiPantScoresHub");

app.Run();
