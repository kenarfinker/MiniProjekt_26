namespace TaskManagerAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }
    }
}
