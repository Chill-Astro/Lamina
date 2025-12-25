Set-Location -Path $PSScriptRoot
Write-Host "Registering Dependencies.." -ForegroundColor Cyan

# 1. Install all MSIX files
Get-ChildItem -Filter *.msix | Sort-Object Name | ForEach-Object {
    Write-Host "Installing: $($_.Name)" -ForegroundColor Yellow
    Add-AppxPackage -Path $_.FullName -ForceApplicationShutdown -ErrorAction SilentlyContinue
}

Write-Host "Dependencies Registered." -ForegroundColor Green
Start-Sleep -Seconds 2

# No Launch here because Inno Setup's [Run] section will handle it from Program Files
exit