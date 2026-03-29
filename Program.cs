using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TaskManagementSystem.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<TaskManagementSystem.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();


builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<TaskManagementSystem.Repositories.IProjectRepository, TaskManagementSystem.Repositories.ProjectRepository>();
builder.Services.AddScoped<TaskManagementSystem.Repositories.ITaskRepository, TaskManagementSystem.Repositories.TaskRepository>();
builder.Services.AddScoped<TaskManagementSystem.Repositories.ICommentRepository, TaskManagementSystem.Repositories.CommentRepository>();
builder.Services.AddScoped<TaskManagementSystem.Repositories.IActivityRepository, TaskManagementSystem.Repositories.ActivityRepository>();
builder.Services.AddScoped<TaskManagementSystem.Repositories.ITaskWatcherRepository, TaskManagementSystem.Repositories.TaskWatcherRepository>();
builder.Services.AddScoped<TaskManagementSystem.Controllers.DashboardController>();

// Add Notification Service
builder.Services.AddScoped<TaskManagementSystem.Services.INotificationService, TaskManagementSystem.Services.NotificationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TaskManagementSystem.Data.ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<TaskManagementSystem.Models.ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await TaskManagementSystem.DbInitializer.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
