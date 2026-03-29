using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TaskManagementSystem.Migrations
{
    public partial class AddMissingTaskColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing columns to Tasks table
            migrationBuilder.AddColumn<int>(
                name: "ActualHours",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Component",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Environment",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedHours",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Labels",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderInBoard",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentTaskId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SprintBoardId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoryPoints",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            // Add foreign key for ParentTaskId
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParentTaskId",
                table: "Tasks",
                column: "ParentTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_ParentTaskId",
                table: "Tasks",
                column: "ParentTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Note: SprintBoards table relationship will be added when SprintBoards table is created
            // For now, just add the column
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SprintBoardId",
                table: "Tasks",
                column: "SprintBoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_ParentTaskId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ParentTaskId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_SprintBoardId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Component",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Environment",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EstimatedHours",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Labels",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "OrderInBoard",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ParentTaskId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SprintBoardId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StoryPoints",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TimeEstimate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TimeSpent",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Tasks");
        }
    }
}
