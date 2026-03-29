using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem
{
    public static class DbInitializer
    {
        public static async System.Threading.Tasks.Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var adminUser = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");


            var user1 = new ApplicationUser { UserName = "user1@example.com", Email = "user1@example.com", EmailConfirmed = true };
            await userManager.CreateAsync(user1, "User@123");
            await userManager.AddToRoleAsync(user1, "User");

            var user2 = new ApplicationUser { UserName = "user2@example.com", Email = "user2@example.com", EmailConfirmed = true };
            await userManager.CreateAsync(user2, "User@123");
            await userManager.AddToRoleAsync(user2, "User");


            var projects = new Project[]
            {
                new Project{Name="Website Redesign", Description="Complete overhaul of the company website.", CreatedDate=DateTime.Now, OwnerId = adminUser.Id},
                new Project{Name="Mobile App Development", Description="Create a new mobile app for iOS and Android.", CreatedDate=DateTime.Now, OwnerId = user1.Id},
            };
            foreach (Project p in projects)
            {
                context.Projects.Add(p);
            }
            await context.SaveChangesAsync();

            var tasks = new Models.Task[]
            {
                new Models.Task{Title="Design new homepage", Description="Create mockups for the new homepage.", Status=Models.TaskStatus.ToDo, Priority=TaskPriority.High, Deadline=DateTime.Now.AddDays(10), ProjectId=projects[0].Id, AssignedUserId=user1.Id, CreatedAt=DateTime.Now},
                new Models.Task{Title="Develop login page", Description="Implement the login functionality.", Status=Models.TaskStatus.InProgress, Priority=TaskPriority.Medium, Deadline=DateTime.Now.AddDays(5), ProjectId=projects[0].Id, AssignedUserId=user2.Id, CreatedAt=DateTime.Now},
                new Models.Task{Title="Setup database", Description="Initialize the database schema for the mobile app.", Status=Models.TaskStatus.Done, Priority=TaskPriority.High, Deadline=DateTime.Now.AddDays(-1), ProjectId=projects[1].Id, AssignedUserId=adminUser.Id, CreatedAt=DateTime.Now},
            };
            foreach (Models.Task t in tasks)
            {
                context.Tasks.Add(t);
            }
            await context.SaveChangesAsync();

            var comments = new Comment[]
            {
                new Comment{Content="Let's use a blue color scheme.", CreatedAt=DateTime.Now, TaskId=tasks[0].Id, UserId=adminUser.Id},
                new Comment{Content="I've started working on the backend.", CreatedAt=DateTime.Now, TaskId=tasks[1].Id, UserId=user2.Id},
            };
            foreach (Comment c in comments)
            {
                context.Comments.Add(c);
            }
            await context.SaveChangesAsync();
        }
    }
}
