namespace FintechCore.Domain.Entities
{
    public class SagaState
    {
        public int Id { get; set; }
        public string SagaId { get; set; } = string.Empty;    // UUID
        public string Status { get; set; } = "Pending";       // Pending, Completed, Failed
        public int CurrentStep { get; set; }
        public string StepData { get; set; } = string.Empty;  // JSON
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
