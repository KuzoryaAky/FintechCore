namespace FintechCore.Domain.Entities
{
    public class Outbox
    {
        public int Id { get; set; }
        public string EventType { get; set; } = string.Empty; // "MoneyTransferred", "AccountCreated"
        public string Payload { get; set; } = string.Empty;   // JSON
        public bool Processed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
