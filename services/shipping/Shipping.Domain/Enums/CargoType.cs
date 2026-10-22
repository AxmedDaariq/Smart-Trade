namespace Shipping.Domain.Enums;

/// <summary>
/// Type of cargo / container used in the shipment.
/// </summary>
public enum CargoType
{
    /// <summary>Full Container Load — shipper fills entire container.</summary>
    FCL,

    /// <summary>Less than Container Load — cargo shares container with others.</summary>
    LCL,

    /// <summary>Bulk cargo not in containers (grain, oil, etc.).</summary>
    Bulk,

    /// <summary>Items that require temperature-controlled transport.</summary>
    Perishable,

    /// <summary>Hazardous materials regulated under IMDG/IATA/ADR codes.</summary>
    Hazardous,

    /// <summary>Oversized cargo exceeding standard container dimensions.</summary>
    BreakBulk,

    /// <summary>Vehicles shipped as cargo.</summary>
    RoRo,

    /// <summary>High-value goods requiring additional security.</summary>
    HighValue,

    /// <summary>General goods not fitting other categories.</summary>
    General
}
