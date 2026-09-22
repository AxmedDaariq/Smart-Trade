using System;

namespace Integration.Domain.Enums;

public enum SystemType
{
    Government,
    Customs,
    Port,
    Bank,
    MobileMoney,
    ECTN,
    TradeInformation,
    TradeMarkAfrica,
    GPSProvider,
    ANPRProvider,
    AISProvider,
    NotificationProvider,
    ERP,
    TMS,
    WMS,
    Other
}

public enum Direction
{
    Inbound,
    Outbound,
    Bidirectional
}

public enum Protocol
{
    REST,
    Webhook,
    SOAP,
    SFTP,
    File,
    AMQP,
    MQTT,
    gRPC,
    Other
}

public enum PartnerStatus
{
    Pending,
    Active,
    Suspended,
    Inactive
}

public enum IntegrationStatus
{
    Draft,
    Active,
    Deprecated,
    Retired
}
