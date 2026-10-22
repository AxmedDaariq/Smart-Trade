using System;

namespace Dryport.Domain.Entities
{
    public class DemurrageRule
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ContainerSize { get; set; }
        public int FreeDays { get; set; }
        public decimal DailyRate { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ContainerRecord
    {
        public Guid Id { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public int Size { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public ContainerStatus Status { get; set; }
        public Guid ClientId { get; set; }
        public Guid DemurrageRuleId { get; set; }
        public DemurrageRule DemurrageRule { get; set; } = null!;
        public decimal AccruedDemurrage { get; set; }
    }

    public enum ContainerStatus
    {
        Arrived,
        InStorage,
        Released,
        Departed
    }

    public class DemurrageAlert
    {
        public Guid Id { get; set; }
        public Guid ContainerRecordId { get; set; }
        public ContainerRecord ContainerRecord { get; set; } = null!;
        public DateTime AlertDate { get; set; }
        public AlertLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
    }

    public enum AlertLevel
    {
        Warning,
        Critical
    }
}
