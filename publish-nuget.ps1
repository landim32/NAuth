# NuGet Package Publishing Script
# This script increments the minor version and publishes NAuth.DTO and NAuth.ACL packages to NuGet

param(
    [string]$ApiKey = "",
    [string]$Source = "https://api.nuget.org/v3/index.json",
    [switch]$SkipBuild = $false,
    [switch]$DryRun = $false
)

# Configuration
$projects = @(
    "NAuth.DTO\NAuth.DTO.csproj",
    "NAuth.ACL\NAuth.ACL.csproj"
)

# Colors for console output
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# Function to get current version from project file
function Get-ProjectVersion {
    param([string]$ProjectPath)
    
    [xml]$projectXml = Get-Content $ProjectPath
    $version = $projectXml.Project.PropertyGroup.Version
    return $version
}

# Function to increment minor version
function Get-IncrementedVersion {
    param([string]$Version)
    
    $versionParts = $Version.Split('.')
    if ($versionParts.Length -lt 3) {
        throw "Invalid version format. Expected format: Major.Minor.Patch"
    }
    
    $major = [int]$versionParts[0]
    $minor = [int]$versionParts[1]
    $patch = [int]$versionParts[2]
    
    # Increment minor version and reset patch to 0
    $minor++
    $patch = 0
    
    return "$major.$minor.$patch"
}

# Function to update version in project file
function Update-ProjectVersion {
    param(
        [string]$ProjectPath,
        [string]$NewVersion
    )
    
    [xml]$projectXml = Get-Content $ProjectPath
    $projectXml.Project.PropertyGroup.Version = $NewVersion
    $projectXml.Save($ProjectPath)
}

# Function to build project
function Build-Project {
    param([string]$ProjectPath)
    
    Write-ColorOutput "Building $ProjectPath..." "Cyan"
    dotnet build $ProjectPath --configuration Release
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed for $ProjectPath"
    }
}

# Function to pack project
function Pack-Project {
    param([string]$ProjectPath)
    
    Write-ColorOutput "Packing $ProjectPath..." "Cyan"
    dotnet pack $ProjectPath --configuration Release --no-build
    
    if ($LASTEXITCODE -ne 0) {
        throw "Pack failed for $ProjectPath"
    }
}

# Function to push package to NuGet
function Push-Package {
    param(
        [string]$PackagePath,
        [string]$ApiKey,
        [string]$Source
    )
    
    if ($DryRun) {
        Write-ColorOutput "DRY RUN: Would push $PackagePath to $Source" "Yellow"
        return
    }
    
    Write-ColorOutput "Pushing $PackagePath to NuGet..." "Cyan"
    dotnet nuget push $PackagePath --api-key $ApiKey --source $Source --skip-duplicate
    
    if ($LASTEXITCODE -ne 0) {
        throw "Push failed for $PackagePath"
    }
}

# Main script execution
try {
    Write-ColorOutput "`n========================================" "Green"
    Write-ColorOutput "NuGet Package Publishing Script" "Green"
    Write-ColorOutput "========================================`n" "Green"
    
    # Validate API Key
    if ([string]::IsNullOrWhiteSpace($ApiKey) -and -not $DryRun) {
        Write-ColorOutput "ERROR: API Key is required. Please provide it using -ApiKey parameter" "Red"
        Write-ColorOutput "Example: .\publish-nuget.ps1 -ApiKey 'your-api-key-here'" "Yellow"
        exit 1
    }
    
    # Process each project
    foreach ($project in $projects) {
        $projectPath = Join-Path $PSScriptRoot $project
        
        if (-not (Test-Path $projectPath)) {
            Write-ColorOutput "WARNING: Project file not found: $projectPath" "Yellow"
            continue
        }
        
        Write-ColorOutput "`n----------------------------------------" "Magenta"
        Write-ColorOutput "Processing: $project" "Magenta"
        Write-ColorOutput "----------------------------------------" "Magenta"
        
        # Get current version
        $currentVersion = Get-ProjectVersion -ProjectPath $projectPath
        Write-ColorOutput "Current version: $currentVersion" "White"
        
        # Increment version
        $newVersion = Get-IncrementedVersion -Version $currentVersion
        Write-ColorOutput "New version: $newVersion" "Green"
        
        if ($DryRun) {
            Write-ColorOutput "DRY RUN: Would update version to $newVersion" "Yellow"
        } else {
            # Update project file with new version
            Update-ProjectVersion -ProjectPath $projectPath -NewVersion $newVersion
            Write-ColorOutput "Version updated in project file" "Green"
        }
        
        # Build project
        if (-not $SkipBuild) {
            Build-Project -ProjectPath $projectPath
            Write-ColorOutput "Build successful" "Green"
        }
        
        # Pack project
        Pack-Project -ProjectPath $projectPath
        Write-ColorOutput "Pack successful" "Green"
        
        # Find the generated .nupkg file
        $projectDir = Split-Path $projectPath -Parent
        $packageName = [System.IO.Path]::GetFileNameWithoutExtension($project)
        $packagePath = Join-Path $projectDir "bin\Release\$packageName.$newVersion.nupkg"
        
        if (-not (Test-Path $packagePath)) {
            Write-ColorOutput "WARNING: Package file not found: $packagePath" "Yellow"
            continue
        }
        
        # Push to NuGet
        Push-Package -PackagePath $packagePath -ApiKey $ApiKey -Source $Source
        Write-ColorOutput "Package published successfully!" "Green"
    }
    
    Write-ColorOutput "`n========================================" "Green"
    Write-ColorOutput "All packages processed successfully!" "Green"
    Write-ColorOutput "========================================`n" "Green"
    
    if ($DryRun) {
        Write-ColorOutput "This was a DRY RUN. No changes were made." "Yellow"
        Write-ColorOutput "Run without -DryRun to actually publish packages." "Yellow"
    }
}
catch {
    Write-ColorOutput "`n========================================" "Red"
    Write-ColorOutput "ERROR: $($_.Exception.Message)" "Red"
    Write-ColorOutput "========================================`n" "Red"
    exit 1
}
