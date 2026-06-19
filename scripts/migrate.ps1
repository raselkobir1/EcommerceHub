#!/usr/bin/env pwsh
<#
.SYNOPSIS
    EF Core migration runner for all EcommerceHub module DbContexts.

.DESCRIPTION
    Runs dotnet ef commands (add, update, list, remove) across every module
    DbContext, or targets a single module via -Module.
    Errors in one module are reported but do not abort the remaining modules.

.PARAMETER Action
    Migration action to perform:
      add     - Add a new migration (requires -Name)
      update  - Apply pending migrations to the database
      list    - List all migrations for the context(s)
      remove  - Remove the last migration for the context(s)

.PARAMETER Name
    Migration name. Required when -Action is 'add'.

.PARAMETER Module
    Optional. Case-insensitive module name to target a single DbContext.
    When omitted, the action is applied to all modules.

.EXAMPLE
    # Apply all pending migrations
    pwsh scripts/migrate.ps1 -Action update

    # Add a new migration to every module
    pwsh scripts/migrate.ps1 -Action add -Name InitialCreate

    # Add a migration to only the Auth module
    pwsh scripts/migrate.ps1 -Action add -Name AddRefreshToken -Module Auth

    # List migrations for the Payments module
    pwsh scripts/migrate.ps1 -Action list -Module Payments

    # Remove the last migration from all modules
    pwsh scripts/migrate.ps1 -Action remove
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('add', 'update', 'list', 'remove')]
    [string]$Action,

    [Parameter(Mandatory = $false)]
    [string]$Name,

    [Parameter(Mandatory = $false)]
    [string]$Module
)

# ---------------------------------------------------------------------------
# Validate inputs
# ---------------------------------------------------------------------------
if ($Action -eq 'add' -and [string]::IsNullOrWhiteSpace($Name)) {
    Write-Host "ERROR: -Name is required when -Action is 'add'." -ForegroundColor Red
    exit 1
}

# ---------------------------------------------------------------------------
# Resolve root paths
# ---------------------------------------------------------------------------
$root           = Split-Path $PSScriptRoot -Parent
$startupProject = "src/EcommerceHub.API/EcommerceHub.API.csproj"

# ---------------------------------------------------------------------------
# Module definitions
# Each entry: ModuleName, DbContextName, Project (path relative to $root)
# ---------------------------------------------------------------------------
$modules = @(
    [ordered]@{
        ModuleName    = 'Auth'
        DbContextName = 'AuthDbContext'
        Project       = 'src/Modules/Auth/EcommerceHub.Modules.Auth/EcommerceHub.Modules.Auth.csproj'
    },
    [ordered]@{
        ModuleName    = 'Catalog'
        DbContextName = 'CatalogDbContext'
        Project       = 'src/Modules/Catalog/EcommerceHub.Modules.Catalog/EcommerceHub.Modules.Catalog.csproj'
    },
    [ordered]@{
        ModuleName    = 'Orders'
        DbContextName = 'OrdersDbContext'
        Project       = 'src/Modules/Orders/EcommerceHub.Modules.Orders/EcommerceHub.Modules.Orders.csproj'
    },
    [ordered]@{
        ModuleName    = 'Cart'
        DbContextName = 'CartDbContext'
        Project       = 'src/Modules/Cart/EcommerceHub.Modules.Cart/EcommerceHub.Modules.Cart.csproj'
    },
    [ordered]@{
        ModuleName    = 'Inventory'
        DbContextName = 'InventoryDbContext'
        Project       = 'src/Modules/Inventory/EcommerceHub.Modules.Inventory/EcommerceHub.Modules.Inventory.csproj'
    },
    [ordered]@{
        ModuleName    = 'Promotions'
        DbContextName = 'PromotionsDbContext'
        Project       = 'src/Modules/Promotions/EcommerceHub.Modules.Promotions/EcommerceHub.Modules.Promotions.csproj'
    },
    [ordered]@{
        ModuleName    = 'Customers'
        DbContextName = 'CustomersDbContext'
        Project       = 'src/Modules/Customers/EcommerceHub.Modules.Customers/EcommerceHub.Modules.Customers.csproj'
    },
    [ordered]@{
        ModuleName    = 'Suppliers'
        DbContextName = 'SuppliersDbContext'
        Project       = 'src/Modules/Suppliers/EcommerceHub.Modules.Suppliers/EcommerceHub.Modules.Suppliers.csproj'
    },
    [ordered]@{
        ModuleName    = 'Reviews'
        DbContextName = 'ReviewsDbContext'
        Project       = 'src/Modules/Reviews/EcommerceHub.Modules.Reviews/EcommerceHub.Modules.Reviews.csproj'
    },
    [ordered]@{
        ModuleName    = 'Payments'
        DbContextName = 'PaymentsDbContext'
        Project       = 'src/Modules/Payments/EcommerceHub.Modules.Payments/EcommerceHub.Modules.Payments.csproj'
    }
)

# ---------------------------------------------------------------------------
# Filter by -Module when specified
# ---------------------------------------------------------------------------
if (-not [string]::IsNullOrWhiteSpace($Module)) {
    $filtered = @($modules | Where-Object { $_.ModuleName -ieq $Module })
    if ($filtered.Count -eq 0) {
        $validNames = ($modules | ForEach-Object { $_.ModuleName }) -join ', '
        Write-Host "ERROR: Module '$Module' not found." -ForegroundColor Red
        Write-Host "Valid modules: $validNames" -ForegroundColor Yellow
        exit 1
    }
    $modules = $filtered
}

# ---------------------------------------------------------------------------
# Execute migrations per module
# ---------------------------------------------------------------------------
$successCount   = 0
$failCount      = 0
$failedModules  = @()

Write-Host ""
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "  EcommerceHub Migration Runner" -ForegroundColor Cyan
Write-Host "  Action  : $Action$(if ($Action -eq 'add') { " '$Name'" } else { '' })" -ForegroundColor Cyan
Write-Host "  Target  : $(if ($Module) { $Module } else { 'ALL modules' })" -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host ""

foreach ($m in $modules) {
    $moduleName  = $m.ModuleName
    $contextName = $m.DbContextName
    $projectPath = "$root/$($m.Project)"
    $startupPath = "$root/$startupProject"

    Write-Host "------------------------------------------------------" -ForegroundColor DarkGray
    Write-Host "  Module : $moduleName" -ForegroundColor Yellow
    Write-Host "  Context: $contextName" -ForegroundColor Yellow
    Write-Host ""

    # Build the dotnet ef argument list for the chosen action
    $efArgs = switch ($Action) {
        'add' {
            @(
                'ef', 'migrations', 'add', $Name,
                '--context',         $contextName,
                '--project',         $projectPath,
                '--startup-project', $startupPath,
                '--output-dir',      'Infrastructure/Persistence/Migrations'
            )
        }
        'update' {
            @(
                'ef', 'database', 'update',
                '--context',         $contextName,
                '--project',         $projectPath,
                '--startup-project', $startupPath
            )
        }
        'list' {
            @(
                'ef', 'migrations', 'list',
                '--context',         $contextName,
                '--project',         $projectPath,
                '--startup-project', $startupPath
            )
        }
        'remove' {
            @(
                'ef', 'migrations', 'remove',
                '--context',         $contextName,
                '--project',         $projectPath,
                '--startup-project', $startupPath
            )
        }
    }

    try {
        & dotnet @efArgs
        $exitCode = $LASTEXITCODE

        if ($exitCode -ne 0) {
            Write-Host ""
            Write-Host "  [FAILED] $moduleName exited with code $exitCode" -ForegroundColor Red
            $failCount++
            $failedModules += $moduleName
        }
        else {
            Write-Host ""
            Write-Host "  [OK] $moduleName" -ForegroundColor Green
            $successCount++
        }
    }
    catch {
        Write-Host ""
        Write-Host "  [ERROR] $moduleName - $_" -ForegroundColor Red
        $failCount++
        $failedModules += $moduleName
    }

    Write-Host ""
}

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "  Summary" -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "  Succeeded : $successCount" -ForegroundColor Green

if ($failCount -gt 0) {
    Write-Host "  Failed    : $failCount  ($($failedModules -join ', '))" -ForegroundColor Red
    Write-Host ""
    Write-Host "One or more modules failed. Review the output above for details." -ForegroundColor Red
    exit 1
}

Write-Host "  Failed    : 0" -ForegroundColor Green
Write-Host ""
Write-Host "All migrations completed successfully." -ForegroundColor Green
exit 0
