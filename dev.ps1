# EcommerceHub — start all three services for local development
# Usage: .\dev.ps1
#        .\dev.ps1 -api   (API only)
#        .\dev.ps1 -front (frontends only)

param(
    [switch]$api,
    [switch]$front
)

$root = $PSScriptRoot

function Stop-Api {
    $procs = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
    if ($procs) {
        $procs | Stop-Process -Force
        Start-Sleep -Seconds 2
        Write-Host "[dev] Stopped existing dotnet processes." -ForegroundColor Yellow
    }
}

function Start-Api {
    Stop-Api
    Write-Host "[dev] Starting API (dotnet watch)..." -ForegroundColor Cyan
    Start-Process "cmd" -ArgumentList "/k dotnet watch run --launch-profile `"EcommerceHub.API`"" `
        -WorkingDirectory "$root\src\EcommerceHub.API"
}

function Start-Frontends {
    Write-Host "[dev] Starting Admin Portal (port 5173)..." -ForegroundColor Cyan
    Start-Process "cmd" -ArgumentList "/k npm run dev" `
        -WorkingDirectory "$root\frontend\admin"

    Write-Host "[dev] Starting Customer Portal (port 3000)..." -ForegroundColor Cyan
    Start-Process "cmd" -ArgumentList "/k npm run dev" `
        -WorkingDirectory "$root\frontend\customer"
}

if ($api) {
    Start-Api
} elseif ($front) {
    Start-Frontends
} else {
    Start-Api
    Start-Sleep -Seconds 3
    Start-Frontends
}

Write-Host ""
Write-Host "[dev] Services launching in separate windows." -ForegroundColor Green
Write-Host "  API:             http://localhost:57972" -ForegroundColor Green
Write-Host "  Admin Portal:    http://localhost:5173" -ForegroundColor Green
Write-Host "  Customer Portal: http://localhost:3000" -ForegroundColor Green
Write-Host "  Swagger:         http://localhost:57972" -ForegroundColor Green
Write-Host ""
Write-Host "Press Ctrl+C in each window to stop." -ForegroundColor DarkGray
