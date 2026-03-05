namespace DevFlow.Application.DTOs.Tasks
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; } = 1;  // 1-4 (Low/Medium/High/Critical)
        public DateTime? DueDate { get; set; }
    }
}