using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TaskManagementSystem.Migrations
{
    public partial class AddNotificationsAndDisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add DisplayName and other new columns to AspNetUsers
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            // Add new columns to Tasks table (من Jira/Trello)
            migrationBuilder.AddColumn<int>(
                name: "OrderInBoard",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Labels",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeEstimate",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeSpent",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Environment",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Component",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            // Create Notifications table
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RelatedTaskId = table.Column<int>(type: "int", nullable: true),
                    RelatedProjectId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TriggeredByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_TriggeredByUserId",
                        column: x => x.TriggeredByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Projects_RelatedProjectId",
                        column: x => x.RelatedProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Tasks_RelatedTaskId",
                        column: x => x.RelatedTaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // Create TaskWatchers table
            migrationBuilder.CreateTable(
                name: "TaskWatchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WatchType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskWatchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskWatchers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskWatchers_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedProjectId",
                table: "Notifications",
                column: "RelatedProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedTaskId",
                table: "Notifications",
                column: "RelatedTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TriggeredByUserId",
                table: "Notifications",
                column: "TriggeredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskWatchers_TaskId_UserId",
                table: "TaskWatchers",
                columns: new[] { "TaskId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskWatchers_UserId",
                table: "TaskWatchers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "TaskWatchers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrderInBoard",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Labels",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TimeEstimate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TimeSpent",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Environment",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Component",
                table: "Tasks");
        }
    }
}
