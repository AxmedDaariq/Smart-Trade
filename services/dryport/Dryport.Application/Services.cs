using System;
using Dryport.Domain.Entities;

namespace Dryport.Application.DTOs
{
    public class ContainerDemurrageDto
    {
        public Guid ContainerId { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int DaysInPort { get; set; }
        public int FreeDays { get; set; }
        public decimal AccruedDemurrage { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    public class ReceiveContainerDto
    {
        public string ContainerNumber { get; set; } = string.Empty;
        public int Size { get; set; }
        public string Type { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public Guid DemurrageRuleId { get; set; }
    }
}

namespace Dryport.Application.Services
{
    public interface IDemurrageService
    {
        decimal CalculateDemurrage(ContainerRecord container, DateTime currentDate);
    }

    public class DemurrageService : IDemurrageService
    {
        public decimal CalculateDemurrage(ContainerRecord container, DateTime currentDate)
        {
            DateTime endDate = container.DepartureDate ?? currentDate;
            int totalDaysInPort = (endDate - container.ArrivalDate).Days;
            
            // Defend against null rule
            if (container.DemurrageRule == null) return 0;

            int chargeableDays = totalDaysInPort - container.DemurrageRule.FreeDays;
            
            if (chargeableDays > 0)
            {
                return chargeableDays * container.DemurrageRule.DailyRate;
            }
            return 0;
        }
    }
}
