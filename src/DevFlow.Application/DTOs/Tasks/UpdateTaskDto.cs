namespace DevFlow.Application.DTOs.Tasks
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Status { get; set; }  // 0=Todo, 1=InProgress, 2=Done
        public int Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}