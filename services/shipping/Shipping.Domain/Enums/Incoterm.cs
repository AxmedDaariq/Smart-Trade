namespace Shipping.Domain.Enums;

/// <summary>
/// Incoterms 2020 — defines obligations, costs and risks between buyer and seller.
/// </summary>
public enum Incoterm
{
    /// <summary>Ex Works — seller makes goods available at their premises.</summary>
    EXW,

    /// <summary>Free Carrier — seller delivers to named place.</summary>
    FCA,

    /// <summary>Carriage Paid To — seller pays freight to destination.</summary>
    CPT,

    /// <summary>Carriage and Insurance Paid To.</summary>
    CIP,

    /// <summary>Delivered at Place — seller delivers to named place of destination.</summary>
    DAP,

    /// <summary>Delivered at Place Unloaded.</summary>
    DPU,

    /// <summary>Delivered Duty Paid — maximum obligation on seller.</summary>
    DDP,

    /// <summary>Free Alongside Ship — sea/inland waterway only.</summary>
    FAS,

    /// <summary>Free On Board — sea/inland waterway only.</summary>
    FOB,

    /// <summary>Cost and Freight — sea/inland waterway only.</summary>
    CFR,

    /// <summary>Cost, Insurance and Freight — sea/inland waterway only.</summary>
    CIF
}
