namespace TaskManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int CompletedTasksThisMonth { get; set; }
        public int TasksInProgress { get; set; }
        public double AvgCompletionRate { get; set; }

        public System.Collections.Generic.List<TaskViewModel> RecentTasks { get; set; } = new();
        public System.Collections.Generic.List<ActivityViewModel> RecentActivities { get; set; } = new();
        public System.Collections.Generic.Dictionary<string, int> TaskDistribution { get; set; } = new();
        public System.Collections.Generic.Dictionary<string, int> MonthlyStats { get; set; } = new();
    }
}
