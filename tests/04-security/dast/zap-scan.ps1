# ═══════════════════════════════════════════════════════════════════════
# Smart Trade Logistics — OWASP ZAP Scanning Script (PowerShell)
# ═══════════════════════════════════════════════════════════════════════

param(
    [string]$TargetUrl = "http://localhost:5000",
    [string]$Mode = "baseline" # "baseline", "api", "full"
)

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host " Starting OWASP ZAP Security Audit for Smart Trade Logistics" -ForegroundColor Cyan
Write-Host " Target URL: $TargetUrl" -ForegroundColor Yellow
Write-Host " Mode:       $Mode" -ForegroundColor Yellow
Write-Host "================================================================" -ForegroundColor Cyan

$ReportsDir = Join-Path $PSScriptRoot "../../09-reports/security"
if (!(Test-Path $ReportsDir)) {
    New-Item -ItemType Directory -Force -Path $ReportsDir | Out-Null
}

node (Join-Path $PSScriptRoot "zap-ci-runner.js") "--mode=$Mode" "--target=$TargetUrl"
