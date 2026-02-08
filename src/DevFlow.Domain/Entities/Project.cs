namespace DevFlow.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Tags { get; set; } = string.Empty; // Stored as comma-separated
        
        // Navigation properties
        public User Owner { get; set; } = null!;
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}