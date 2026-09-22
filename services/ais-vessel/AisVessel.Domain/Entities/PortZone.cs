using System;
using NetTopologySuite.Geometries;
using AisVessel.Domain.Enums;

namespace AisVessel.Domain.Entities;

public class PortZone
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string PortCode { get; private set; }
    public PortZoneType Type { get; private set; }
    public string Name { get; private set; }
    
    // PostGIS Geometry for Polygon/MultiPolygon
    public Geometry Boundary { get; private set; }
    
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] Version { get; private set; }

    protected PortZone() { }

    public PortZone(
        Guid tenantId,
        string portCode,
        string name,
        PortZoneType type,
        Geometry boundary)
    {
        if (boundary == null) throw new ArgumentNullException(nameof(boundary));
        
        // Ensure SRID is set
        if (boundary.SRID == 0) boundary.SRID = 4326;

        Id = Guid.NewGuid();
        TenantId = tenantId;
        PortCode = portCode;
        Name = name;
        Type = type;
        Boundary = boundary;
        Active = true;
        CreatedAt = DateTime.UtcNow;
    }
}
