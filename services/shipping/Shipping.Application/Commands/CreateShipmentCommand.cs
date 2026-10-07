using MediatR;
using FluentValidation;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;

namespace Shipping.Application.Commands;

// ─────────────────────────────────────────────────────────────────────────────
// Command
// ─────────────────────────────────────────────────────────────────────────────

public class CreateShipmentCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }

    // Parties
    public string ShipperName { get; set; } = string.Empty;
    public string ShipperReference { get; set; } = string.Empty;
    public string ConsigneeName { get; set; } = string.Empty;
    public string ConsigneeReference { get; set; } = string.Empty;
    public string? CarrierCode { get; set; }
    public string? FreightForwarderCode { get; set; }

    // Route
    public string OriginCountryCode { get; set; } = string.Empty;
    public string OriginPortCode { get; set; } = string.Empty;
    public string OriginPortName { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;
    public string DestinationPortCode { get; set; } = string.Empty;
    public string DestinationPortName { get; set; } = string.Empty;

    // Cargo
    public TransportMode TransportMode { get; set; }
    public CargoType CargoType { get; set; }
    public Incoterm Incoterm { get; set; }
    public string Currency { get; set; } = "USD";
    public string? HsCode { get; set; }
    public string? GoodsDescription { get; set; }
    public decimal GrossWeightKg { get; set; }
    public decimal NetWeightKg { get; set; }
    public decimal VolumeM3 { get; set; }
    public int PackageCount { get; set; }
    public decimal? DeclaredValue { get; set; }

    // Dates
    public DateTime? EstimatedDepartureUtc { get; set; }
    public DateTime? EstimatedArrivalUtc { get; set; }

    // Line items
    public List<CreateShipmentLineDto> Lines { get; set; } = new();

    // Created by
    public Guid UserId { get; set; }
}

public class CreateShipmentLineDto
{
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string? CountryOfOrigin { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = "PCS";
    public decimal GrossWeightKg { get; set; }
    public decimal NetWeightKg { get; set; }
    public decimal UnitValue { get; set; }
    public decimal TotalValue { get; set; }
    public string Currency { get; set; } = "USD";
}

// ─────────────────────────────────────────────────────────────────────────────
// Validator
// ─────────────────────────────────────────────────────────────────────────────

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ShipperName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShipperReference).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ConsigneeName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ConsigneeReference).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OriginCountryCode).NotEmpty().Length(2, 3);
        RuleFor(x => x.OriginPortCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.DestinationCountryCode).NotEmpty().Length(2, 3);
        RuleFor(x => x.DestinationPortCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.GrossWeightKg).GreaterThan(0);
        RuleFor(x => x.PackageCount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.UserId).NotEmpty();
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// DB Context Interface
// ─────────────────────────────────────────────────────────────────────────────

public interface IShippingDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void AddShipment(Shipment shipment);
}

// ─────────────────────────────────────────────────────────────────────────────
// Shipment Number Generator
// ─────────────────────────────────────────────────────────────────────────────

public interface IShipmentNumberGenerator
{
    string Generate(Guid tenantId);
}

// ─────────────────────────────────────────────────────────────────────────────
// Handler
// ─────────────────────────────────────────────────────────────────────────────

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Guid>
{
    private readonly IShippingDbContext _context;
    private readonly IShipmentNumberGenerator _numberGenerator;

    public CreateShipmentCommandHandler(
        IShippingDbContext context,
        IShipmentNumberGenerator numberGenerator)
    {
        _context = context;
        _numberGenerator = numberGenerator;
    }

    public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipmentNumber = _numberGenerator.Generate(request.TenantId);

        var shipment = new Shipment(
            tenantId: request.TenantId,
            shipmentNumber: shipmentNumber,
            shipperName: request.ShipperName,
            shipperReference: request.ShipperReference,
            consigneeName: request.ConsigneeName,
            consigneeReference: request.ConsigneeReference,
            originCountryCode: request.OriginCountryCode,
            originPortCode: request.OriginPortCode,
            originPortName: request.OriginPortName,
            destinationCountryCode: request.DestinationCountryCode,
            destinationPortCode: request.DestinationPortCode,
            destinationPortName: request.DestinationPortName,
            transportMode: request.TransportMode,
            cargoType: request.CargoType,
            incoterm: request.Incoterm,
            currency: request.Currency,
            createdBy: request.UserId
        );

        shipment.OrganizationId = request.OrganizationId;
        shipment.CarrierCode = request.CarrierCode;
        shipment.FreightForwarderCode = request.FreightForwarderCode;
        shipment.HsCode = request.HsCode;
        shipment.GoodsDescription = request.GoodsDescription;
        shipment.GrossWeightKg = request.GrossWeightKg;
        shipment.NetWeightKg = request.NetWeightKg;
        shipment.VolumeM3 = request.VolumeM3;
        shipment.PackageCount = request.PackageCount;
        shipment.DeclaredValue = request.DeclaredValue;
        shipment.EstimatedDepartureUtc = request.EstimatedDepartureUtc;
        shipment.EstimatedArrivalUtc = request.EstimatedArrivalUtc;

        // Add cargo lines
        foreach (var lineDto in request.Lines)
        {
            shipment.Lines.Add(new ShipmentLine
            {
                ShipmentId = shipment.Id,
                LineNumber = lineDto.LineNumber,
                Description = lineDto.Description,
                HsCode = lineDto.HsCode,
                CountryOfOrigin = lineDto.CountryOfOrigin,
                Quantity = lineDto.Quantity,
                UnitOfMeasure = lineDto.UnitOfMeasure,
                GrossWeightKg = lineDto.GrossWeightKg,
                NetWeightKg = lineDto.NetWeightKg,
                UnitValue = lineDto.UnitValue,
                TotalValue = lineDto.TotalValue,
                Currency = lineDto.Currency
            });
        }

        _context.AddShipment(shipment);

        // TODO: Publish ShipmentCreatedEvent to outbox

        await _context.SaveChangesAsync(cancellationToken);

        return shipment.Id;
    }
}
