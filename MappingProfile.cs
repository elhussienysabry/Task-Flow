using AutoMapper;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModels;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectViewModel>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.UserName))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count))
                .ForMember(dest => dest.CompletedTaskCount, opt => opt.MapFrom(src => src.Tasks.Count(t => t.Status == Models.TaskStatus.Done)))
                .ForMember(dest => dest.CompletionPercentage, opt => opt.MapFrom(src => src.Tasks.Any() ? (double)src.Tasks.Count(t => t.Status == Models.TaskStatus.Done) / src.Tasks.Count * 100 : 0));
            CreateMap<ProjectViewModel, Project>();

            CreateMap<Task, TaskViewModel>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.UserName : ""));
            CreateMap<TaskViewModel, Task>();

            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.ProfileImageUrl));
            CreateMap<CommentViewModel, Comment>();

            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();

            CreateMap<Activity, ActivityViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.TaskTitle, opt => opt.MapFrom(src => src.Task != null ? src.Task.Title : ""))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : ""));
            CreateMap<ActivityViewModel, Activity>();
        }
    }
}
