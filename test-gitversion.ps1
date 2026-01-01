# GitVersion Local Test Script
# This script helps you test how GitVersion will calculate versions
# without actually committing changes

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   GitVersion Local Testing Tool" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Check if gitversion is installed
Write-Host "Checking GitVersion installation..." -ForegroundColor Yellow
$gitversionInstalled = Get-Command dotnet-gitversion -ErrorAction SilentlyContinue

if (-not $gitversionInstalled) {
    Write-Host "GitVersion not found. Installing..." -ForegroundColor Red
    dotnet tool install --global GitVersion.Tool
    Write-Host "GitVersion installed successfully!" -ForegroundColor Green
    Write-Host ""
}

# Function to display version info
function Show-VersionInfo {
    Write-Host ""
    Write-Host "Current Version Information:" -ForegroundColor Green
    Write-Host "------------------------------------" -ForegroundColor Gray
    
    $version = dotnet-gitversion 2>$null | ConvertFrom-Json
    
    if ($version) {
        Write-Host "  Version (SemVer):        " -NoNewline; Write-Host $version.SemVer -ForegroundColor Cyan
        Write-Host "  NuGet Version:           " -NoNewline; Write-Host $version.NuGetVersionV2 -ForegroundColor Cyan
        Write-Host "  Major.Minor.Patch:       " -NoNewline; Write-Host $version.MajorMinorPatch -ForegroundColor Cyan
        Write-Host "  Full SemVer:             " -NoNewline; Write-Host $version.FullSemVer -ForegroundColor Cyan
        Write-Host "  Assembly Version:        " -NoNewline; Write-Host $version.AssemblySemVer -ForegroundColor Cyan
        Write-Host "  Branch:                  " -NoNewline; Write-Host $version.BranchName -ForegroundColor Yellow
        Write-Host "  Commit SHA:              " -NoNewline; Write-Host $version.ShortSha -ForegroundColor Gray
        Write-Host "  Commits Since Version:   " -NoNewline; Write-Host $version.CommitsSinceVersionSource -ForegroundColor Gray
        
        if ($version.PreReleaseLabel) {
            Write-Host "  Pre-Release Label:       " -NoNewline; Write-Host $version.PreReleaseLabel -ForegroundColor Magenta
        }
    }
    else {
        Write-Host "Error calculating version. Check GitVersion.yml configuration." -ForegroundColor Red
    }
    Write-Host ""
}

# Function to simulate commit
function Test-CommitMessage {
    param (
        [string]$message
    )
    
    Write-Host ""
    Write-Host "Testing commit message: '$message'" -ForegroundColor Yellow
    Write-Host "------------------------------------" -ForegroundColor Gray
    
    # Show current version BEFORE change
    Write-Host ""
    Write-Host "  Current Version (Before): " -NoNewline
    $versionBefore = dotnet-gitversion 2>$null | ConvertFrom-Json
    if ($versionBefore) {
        Write-Host $versionBefore.NuGetVersionV2 -ForegroundColor Yellow
        Write-Host "  Current SemVer:           " -NoNewline
        Write-Host $versionBefore.SemVer -ForegroundColor Yellow
    }
    
    # Create temporary commit
    git add -A 2>$null
    git commit -m "$message" --allow-empty 2>$null | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        # Show version AFTER change
        $versionAfter = dotnet-gitversion 2>$null | ConvertFrom-Json
        
        if ($versionAfter) {
            Write-Host ""
            Write-Host "  New Version (After):      " -NoNewline
            Write-Host $versionAfter.NuGetVersionV2 -ForegroundColor Green
            Write-Host "  New SemVer:               " -NoNewline
            Write-Host $versionAfter.SemVer -ForegroundColor Green
            
            # Show change indicator
            if ($versionBefore -and $versionAfter) {
                Write-Host ""
                if ($versionBefore.NuGetVersionV2 -ne $versionAfter.NuGetVersionV2) {
                    Write-Host "  Change: " -NoNewline
                    Write-Host "$($versionBefore.NuGetVersionV2) ? $($versionAfter.NuGetVersionV2)" -ForegroundColor Cyan
                } else {
                    Write-Host "  No version change detected" -ForegroundColor Magenta
                }
            }
        }
        
        # Undo commit
        git reset --soft HEAD~1 2>$null | Out-Null
        git reset 2>$null | Out-Null
    }
    else {
        Write-Host "  Could not create test commit" -ForegroundColor Red
    }
}

# Main menu
function Show-Menu {
    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "   What would you like to test?" -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Show current version" -ForegroundColor White
    Write-Host "2. Test PATCH increment (fix: message)" -ForegroundColor White
    Write-Host "3. Test MINOR increment (feature: message)" -ForegroundColor White
    Write-Host "4. Test MAJOR increment (breaking: message)" -ForegroundColor White
    Write-Host "5. Test custom commit message" -ForegroundColor White
    Write-Host "6. Show Git history" -ForegroundColor White
    Write-Host "7. Show commits since last tag" -ForegroundColor White
    Write-Host "8. Export full version JSON" -ForegroundColor White
    Write-Host "9. Run GitVersion with diagnostics" -ForegroundColor White
    Write-Host "0. Exit" -ForegroundColor White
    Write-Host ""
}

# Main loop
do {
    Show-Menu
    $choice = Read-Host "Enter your choice"
    
    switch ($choice) {
        "1" {
            Show-VersionInfo
            Read-Host "Press Enter to continue"
        }
        "2" {
            Test-CommitMessage "fix: Test patch increment"
            Read-Host "Press Enter to continue"
        }
        "3" {
            Test-CommitMessage "feature: Test minor increment"
            Read-Host "Press Enter to continue"
        }
        "4" {
            Test-CommitMessage "breaking: Test major increment"
            Read-Host "Press Enter to continue"
        }
        "5" {
            $customMessage = Read-Host "Enter your custom commit message"
            Test-CommitMessage $customMessage
            Read-Host "Press Enter to continue"
        }
        "6" {
            Write-Host ""
            Write-Host "Recent Git History:" -ForegroundColor Green
            Write-Host "------------------------------------" -ForegroundColor Gray
            git log --oneline --decorate --graph -20
            Write-Host ""
            Read-Host "Press Enter to continue"
        }
        "7" {
            Write-Host ""
            Write-Host "Commits since last tag:" -ForegroundColor Green
            Write-Host "------------------------------------" -ForegroundColor Gray
            
            $lastTag = git describe --tags --abbrev=0 2>$null
            if ($lastTag) {
                Write-Host "Last tag: $lastTag" -ForegroundColor Yellow
                Write-Host ""
                git log "$lastTag..HEAD" --oneline --decorate
            }
            else {
                Write-Host "No tags found in repository" -ForegroundColor Yellow
                Write-Host ""
                git log --oneline --decorate -20
            }
            Write-Host ""
            Read-Host "Press Enter to continue"
        }
        "8" {
            Write-Host ""
            Write-Host "Full Version JSON:" -ForegroundColor Green
            Write-Host "------------------------------------" -ForegroundColor Gray
            dotnet-gitversion | ConvertFrom-Json | ConvertTo-Json -Depth 10
            Write-Host ""
            Read-Host "Press Enter to continue"
        }
        "9" {
            Write-Host ""
            Write-Host "Running GitVersion with diagnostics..." -ForegroundColor Green
            Write-Host "------------------------------------" -ForegroundColor Gray
            dotnet-gitversion /verbosity Diagnostic
            Write-Host ""
            Read-Host "Press Enter to continue"
        }
        "0" {
            Write-Host ""
            Write-Host "Goodbye!" -ForegroundColor Green
            Write-Host ""
        }
        default {
            Write-Host ""
            Write-Host "Invalid choice. Please try again." -ForegroundColor Red
            Start-Sleep -Seconds 2
        }
    }
    
    Clear-Host
} while ($choice -ne "0")
