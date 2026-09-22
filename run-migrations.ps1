$ErrorActionPreference = "Continue"

$services = @(
    "identity", "payment", "customs", "fleet", "declaration", 
    "ectn", "corridor", "gps-telematics", "ais-vessel", "anpr-gate", 
    "audit", "notification", "integration", "analytics"
)

foreach ($svc in $services) {
    Write-Host "========================================="
    Write-Host "Processing Service: $svc"
    
    # Capitalize first letter to match project naming conventions usually
    $SvcCapital = (Get-Culture).TextInfo.ToTitleCase($svc)
    
    if ($svc -eq "gps-telematics") { $SvcCapital = "GpsTelematics" }
    if ($svc -eq "ais-vessel") { $SvcCapital = "AisVessel" }
    if ($svc -eq "anpr-gate") { $SvcCapital = "AnprGate" }
    
    $apiProject = "services/$svc/$SvcCapital.Api"
    $infraProject = "services/$svc/$SvcCapital.Infrastructure"
    
    if ((Test-Path $apiProject) -and (Test-Path $infraProject)) {
        Write-Host "Adding Migration for $svc..."
        dotnet ef migrations add InitialCreate --project $infraProject --startup-project $apiProject --output-dir Persistence/Migrations
        
        Write-Host "Updating Database for $svc..."
        dotnet ef database update --project $infraProject --startup-project $apiProject
    } else {
        Write-Host "Skipping $svc - Project paths not found"
    }
}
Write-Host "Done!"
