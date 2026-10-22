using System;

namespace Declaration.Domain.Entities;

public class DeclarationPackage
{
    public Guid Id { get; private set; }
    public Guid DeclarationId { get; private set; }
    public string PackageNumber { get; private set; }
    public string PackageType { get; private set; }
    public decimal Quantity { get; private set; }
    public string MarksAndNumbers { get; private set; }
    public decimal Weight { get; private set; }
    public string WeightUnit { get; private set; }

    protected DeclarationPackage() { }

    public DeclarationPackage(Guid declarationId, string packageType, decimal quantity)
    {
        Id = Guid.NewGuid();
        DeclarationId = declarationId;
        PackageType = packageType;
        Quantity = quantity;
    }
}
