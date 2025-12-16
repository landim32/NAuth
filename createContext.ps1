# Load environment variables from .env file
$envFile = ".env"

if (-Not (Test-Path $envFile)) {
    Write-Host "Error: .env file not found!" -ForegroundColor Red
    Write-Host "Please create a .env file based on .env.example" -ForegroundColor Yellow
    exit 1
}

# Read .env file and parse CONNECTION_STRING
$connectionString = $null
Get-Content $envFile | ForEach-Object {
    if ($_ -match '^CONNECTION_STRING=(.+)$') {
        $connectionString = $matches[1].Trim()
    }
}

if ([string]::IsNullOrEmpty($connectionString)) {
    Write-Host "Error: CONNECTION_STRING not found in .env file!" -ForegroundColor Red
    exit 1
}

Write-Host "Using connection string from .env file" -ForegroundColor Green
Write-Host "Connection: $connectionString" -ForegroundColor Cyan

# Navigate to NAuth.Infra directory
Set-Location -Path ".\NAuth.Infra"

# Run dotnet ef dbcontext scaffold
Write-Host "Scaffolding database context..." -ForegroundColor Yellow
dotnet ef dbcontext scaffold "$connectionString" Npgsql.EntityFrameworkCore.PostgreSQL --context NAuthContext --output-dir Context -f

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database context scaffolded successfully!" -ForegroundColor Green
} else {
    Write-Host "Error scaffolding database context!" -ForegroundColor Red
}

# Return to original directory
Set-Location -Path ".."

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
