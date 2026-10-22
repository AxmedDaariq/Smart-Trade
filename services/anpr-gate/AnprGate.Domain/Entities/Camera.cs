using System;
using NetTopologySuite.Geometries;
using AnprGate.Domain.Enums;

namespace AnprGate.Domain.Entities;

public class Camera
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string CameraCode { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    // PostGIS location
    public Geometry Location { get; private set; }
    
    public string PortOrBorderReference { get; private set; }
    public Guid? GateId { get; private set; }
    public Guid? LaneId { get; private set; }
    
    public Direction Direction { get; private set; }
    public CameraType CameraType { get; private set; }
    
    public string Manufacturer { get; private set; }
    public string Model { get; private set; }
    public string SerialNumber { get; private set; }
    public string FirmwareVersion { get; private set; }
    
    public CameraStatus Status { get; private set; }
    public DateTime? LastHeartbeatAt { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] Version { get; private set; }

    protected Camera() { }

    public Camera(Guid tenantId, string cameraCode, string name, CameraType type, Geometry location)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CameraCode = cameraCode;
        Name = name;
        CameraType = type;
        
        if (location != null && location.SRID == 0) location.SRID = 4326;
        Location = location;
        
        Status = CameraStatus.Provisioning;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateHeartbeat()
    {
        LastHeartbeatAt = DateTime.UtcNow;
        if (Status == CameraStatus.Offline)
        {
            Status = CameraStatus.Active;
        }
    }
}
