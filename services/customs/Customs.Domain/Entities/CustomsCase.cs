using System;
using System.Collections.Generic;

namespace Customs.Domain.Entities;

public class CustomsCase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    
    public string CaseNumber { get; set; } = string.Empty;
    public Guid DeclarationId { get; set; }
    public string DeclarationExternalId { get; set; } = string.Empty;
    public string DeclarationNumber { get; set; } = string.Empty;
    public int DeclarationVersionNumber { get; set; }
    public string? ECTNExternalId { get; set; }
    public string? EctnNumber { get; set; }
    
    public string ProcedureCode { get; set; } = string.Empty;
    public string CaseType { get; set; } = "Import"; // Import, Export, Transit, ReExport, TemporaryImport, TemporaryExport, FreeZone, Warehousing, Other
    
    public string ImporterReference { get; set; } = string.Empty;
    public string ExporterReference { get; set; } = string.Empty;
    public string DeclarantReference { get; set; } = string.Empty;
    public string PortOfEntry { get; set; } = string.Empty;
    public string PortOfExit { get; set; } = string.Empty;
    public string CountryOfOrigin { get; set; } = string.Empty;
    public string CountryOfDestination { get; set; } = string.Empty;

    public string Status { get; set; } = "Received"; // Received, ValidationPending, RiskAssessment, Assessment, InspectionPending, Inspection, DutyAssessed, PaymentPending, PaymentConfirmed, ClearanceReview, Cleared, ReleaseAuthorized, Released, Held, Rejected, Cancelled, Reassessment
    
    public DateTime OpenedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<CustomsReference> References { get; set; } = new List<CustomsReference>();
    public ICollection<CustomsAssessment> Assessments { get; set; } = new List<CustomsAssessment>();
    public ICollection<CustomsValuation> Valuations { get; set; } = new List<CustomsValuation>();
    public ICollection<CustomsHold> Holds { get; set; } = new List<CustomsHold>();
    public ICollection<CustomsDiscrepancy> Discrepancies { get; set; } = new List<CustomsDiscrepancy>();
    public ICollection<CustomsDecision> Decisions { get; set; } = new List<CustomsDecision>();
    public ICollection<CustomsStatusHistory> StatusHistories { get; set; } = new List<CustomsStatusHistory>();

    public CustomsCase() { }

    public CustomsCase(
        Guid tenantId,
        string declarationExternalId,
        string declarationNumber,
        string ectnExternalId,
        string ectnNumber,
        string caseNumber,
        string procedureCode,
        string importerReference,
        string exporterReference,
        string declarantReference,
        string portOfEntry,
        string portOfExit,
        string countryOfOrigin,
        string countryOfDestination,
        Guid userId)
    {
        TenantId = tenantId;
        DeclarationExternalId = declarationExternalId ?? string.Empty;
        DeclarationNumber = declarationNumber ?? string.Empty;
        ECTNExternalId = ectnExternalId;
        EctnNumber = ectnNumber;
        CaseNumber = caseNumber ?? string.Empty;
        ProcedureCode = procedureCode ?? string.Empty;
        ImporterReference = importerReference ?? string.Empty;
        ExporterReference = exporterReference ?? string.Empty;
        DeclarantReference = declarantReference ?? string.Empty;
        PortOfEntry = portOfEntry ?? string.Empty;
        PortOfExit = portOfExit ?? string.Empty;
        CountryOfOrigin = countryOfOrigin ?? string.Empty;
        CountryOfDestination = countryOfDestination ?? string.Empty;
        CreatedBy = userId;
        Status = "Received";
        OpenedAtUtc = DateTime.UtcNow;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
