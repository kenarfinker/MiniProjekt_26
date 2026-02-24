namespace TaskManagerAPI.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public ICollection<Project> OwnedProjects { get; set; }
        public ICollection<TaskItem> AssignedTasks { get; set; }
    }
}
