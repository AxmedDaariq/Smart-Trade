#!/bin/bash
set -e

services=("identity" "payment" "customs" "fleet" "declaration" "ectn" "corridor" "gps-telematics" "ais-vessel" "anpr-gate" "audit" "notification" "integration" "analytics")

for svc in "${services[@]}"; do
    echo "========================================="
    echo "Processing Service: $svc"
    
    # Capitalize the service name to match folder names (e.g., identity -> Identity)
    if [ "$svc" == "gps-telematics" ]; then
        svc_capital="GpsTelematics"
    elif [ "$svc" == "ais-vessel" ]; then
        svc_capital="AisVessel"
    elif [ "$svc" == "anpr-gate" ]; then
        svc_capital="AnprGate"
    else
        # Capitalize first letter
        svc_capital="$(tr '[:lower:]' '[:upper:]' <<< ${svc:0:1})${svc:1}"
    fi

    apiProject="services/$svc/$svc_capital.Api"
    infraProject="services/$svc/$svc_capital.Infrastructure"
    
    if [ -d "$apiProject" ] && [ -d "$infraProject" ]; then
        echo "Adding Migration for $svc..."
        dotnet ef migrations add InitialCreate --project $infraProject --startup-project $apiProject --output-dir Persistence/Migrations || true
        
        echo "Updating Database for $svc..."
        dotnet ef database update --project $infraProject --startup-project $apiProject || true
    else
        echo "Skipping $svc - Project paths not found"
    fi
done

echo "Done!"
