# TaskFlow - Team Task Management System

A modern, action-packed task management application built with ASP.NET Core 8.0, Entity Framework Core, and Bootstrap 5. Perfect for organizing projects, managing tasks, and collaborating with team members.

## Features

###  Authentication & Authorization
- Complete user authentication using ASP.NET Core Identity
- Register, Login, and Logout functionality
- Role-based access control (Admin, User)
- Secure password management
- Authorization checks on all protected resources

###  Project Management
- Create, edit, and delete projects
- View all your projects with search functionality
- Project completion percentage progress bars
- Only project owners can edit/delete their projects
- Cascade delete related tasks when project is deleted

###  Task Management
- Full CRUD operations for tasks within projects
- Task properties:
  - **Title & Description**: Clear task identification
  - **Status**: To Do, In Progress, Done
  - **Priority**: Low, Medium, High
  - **Deadline**: Track when tasks are due
  - **Assignment**: Assign tasks to team members
  - **Comments**: Collaborate with task comments

###  Dashboard
- Overview of your workload:
  - Total projects count
  - Total tasks count
  - Overdue tasks count
  - Task distribution chart (To Do, In Progress, Done)
  - Recent tasks list
- Visual statistics with icons and cards
- Real-time data updates

###  Search & Filter
- Search projects by name
- Search tasks by title
- Filter tasks by status
- Filter tasks by priority
- Combined search and filter functionality

###  Comments System
- Add comments to tasks
- View all comments with timestamps
- Display commenter information
- Ordered by most recent first
- Direct feedback on task progress

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core (Code-First)
- **ORM**: Entity Framework Core with migrations
- **Authentication**: ASP.NET Core Identity
- **Mapping**: AutoMapper
- **Validation**: Server-side & Client-side validation

### Frontend
- **UI Framework**: Bootstrap 5
- **Icons**: Font Awesome 5.15.4
- **Alerts**: SweetAlert2
- **JavaScript**: jQuery with unobtrusive validation
- **Charts**: Chart.js for dashboard visualization

## Project Structure

```
TaskManagementSystem/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs
│   ├── DashboardController.cs
│   ├── ProjectsController.cs
│   └── TasksController.cs
├── Views/                # Razor Views
│   ├── Account/
│   ├── Dashboard/
│   ├── Projects/
│   ├── Tasks/
│   └── Shared/
├── Models/               # Database Models/Entities
│   ├── ApplicationUser.cs
│   ├── Project.cs
│   ├── Task.cs
│   ├── Comment.cs
│   ├── TaskStatus.cs
│   └── TaskPriority.cs
├── ViewModels/          # View-specific Models
├── Repositories/        # Data Access Layer
├── Data/               # DbContext & Migrations
├── wwwroot/            # Static files (CSS, JS, Images)
└── Program.cs          # Application configuration

```

## Database Schema

### Users Table (Extended from Identity)
- Id (PK)
- UserName, Email
- Projects (1:N)
- Tasks (1:N)
- Comments (1:N)

### Projects Table
- Id (PK)
- Name, Description
- CreatedDate
- OwnerId (FK to Users)
- Tasks (1:N)

### Tasks Table
- Id (PK)
- Title, Description
- Status (Enum: ToDo, InProgress, Done)
- Priority (Enum: Low, Medium, High)
- Deadline (DateTime)
- ProjectId (FK)
- AssignedUserId (FK to Users)
- CreatedAt
- Comments (1:N)

### Comments Table
- Id (PK)
- Content
- CreatedAt
- TaskId (FK)
- UserId (FK to Users)

## Entity Relationships

```
User (1) -------- * (Projects)    : One user can own many projects
User (1) -------- * (Tasks)       : One user can be assigned many tasks
User (1) -------- * (Comments)    : One user can write many comments
Project (1) ---- * (Tasks)        : One project contains many tasks
Task (1) -------- * (Comments)    : One task has many comments
```

## Installation & Setup

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Steps

1. **Clone or extract the repository**
   ```bash
   cd TaskManagementSystem
   ```

2. **Configure Database Connection**
   - Open `appsettings.json`
   - Update the `DefaultConnection` string if needed:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementSystem;Trusted_Connection=True;"
   }
   ```

3. **Install Dependencies**
   ```bash
   dotnet restore
   ```

4. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```
   Or using Package Manager Console:
   ```powershell
   Update-Database
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```
   The application will be available at: `https://localhost:7xxx`


### Sample Projects & Tasks
- **Website Redesign** (Admin's project)
  - Design new homepage
  - Develop login page
  
- **Mobile App Development** (User1's project)
  - Setup database

## Key Features Explained

### Authorization
- Users can only view/manage their own projects
- Only project owners can edit or delete projects
- Users can see tasks assigned to them or in their projects

### Task Overdue Detection
- Tasks with deadline < today and status ≠ Done are marked as overdue
- Visual indicators on both task lists and details pages
- Overdue tasks appear in the dashboard count

### Progress Tracking
- Project completion percentage calculated automatically
- Progress bar visualization on project cards
- Task distribution chart on dashboard

### Comments System
- Comments are displayed in task details page
- Sorted by most recent first
- Shows commenter name and timestamp
- Simple text-based commenting

## API/Routes

### Account Routes
- `GET /Account/Login` - Login page
- `POST /Account/Login` - Login submission
- `GET /Account/Register` - Register page
- `POST /Account/Register` - Register submission
- `POST /Account/Logout` - Logout

### Project Routes
- `GET /Projects/Index` - List all user's projects
- `GET /Projects/Create` - Create project form
- `POST /Projects/Create` - Create project submission
- `GET /Projects/Edit/{id}` - Edit project form
- `POST /Projects/Edit/{id}` - Edit project submission
- `POST /Projects/Delete/{id}` - Delete project

### Task Routes
- `GET /Tasks/Index?projectId={id}` - List project tasks
- `GET /Tasks/Create?projectId={id}` - Create task form
- `POST /Tasks/Create` - Create task submission
- `GET /Tasks/Edit/{id}` - Edit task form
- `POST /Tasks/Edit/{id}` - Edit task submission
- `GET /Tasks/Details/{id}` - View task details
- `POST /Tasks/Delete/{id}` - Delete task
- `POST /Tasks/AddComment` - Add comment to task

### Dashboard
- `GET /Dashboard/Index` - User dashboard

### Home
- `GET /Home/Index` - Landing page / Home

## Validation

### Server-Side Validation
- Required field validation
- String length validation
- Email format validation
- Password strength validation
- Authorization checks

### Client-Side Validation
- jQuery Validation
- HTML5 form validation
- Real-time field validation
- Error messages display

## Security Features

- **Authentication**: ASP.NET Core Identity
- **Authorization**: Role-based and resource-based authorization
- **CSRF Protection**: AntiForgery tokens on all forms
- **SQL Injection Prevention**: Entity Framework parameterized queries
- **Password Security**: Hashed and salted passwords
- **HTTPS**: Enabled in production

## UI/UX Features

### Modern Design
- Clean, light-themed interface
- Gradient backgrounds and shadows
- Smooth animations and transitions
- Responsive design (mobile, tablet, desktop)

### Interactive Elements
- Hover effects on cards and buttons
- SweetAlert2 confirmation dialogs
- Progress bars with percentages
- Status and priority badges
- Icon-rich interface using Font Awesome

### Accessibility
- Semantic HTML
- ARIA labels and roles
- Keyboard navigation support
- Color contrast compliance
- Clear form labels

## Performance Optimizations

- Async/await for all database operations
- Entity Framework lazy loading considerations
- Efficient LINQ queries
- Bootstrap CDN for fast CSS loading
- Minified CSS and JavaScript
- Optimized database queries with proper indexing

## Error Handling

- Custom error pages
- User-friendly error messages
- Server-side exception logging
- Client-side form validation
- Graceful degradation

## Future Enhancements

1. **SignalR Integration**: Real-time comment notifications
2. **Email Notifications**: Notify users of task assignments and deadlines
3. **File Attachments**: Attach files to tasks
4. **Activity Log**: Track all changes to projects and tasks
5. **Pagination**: Implement pagination for large lists
6. **Advanced Filtering**: More complex filter combinations
7. **Recurring Tasks**: Support for recurring task creation
8. **Task Dependencies**: Define task dependencies and workflows
9. **Team Collaboration**: Share projects with team members
10. **Mobile App**: Native mobile application

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure LocalDB is installed (comes with Visual Studio)

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove

# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

### Port Already in Use
- Change the port in `Properties/launchSettings.json`
- Or use: `dotnet run --urls="https://localhost:8443"`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is open source and available under the MIT License.

## Support

For issues and questions, please open an issue in the repository.

---

**Built with El-Hussieny El-Nemr❤️ using ASP.NET Core**
