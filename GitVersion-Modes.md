# GitVersion Mode Explanation

## Why ContinuousDelivery Mode?

The project was changed from **ContinuousDeployment** to **ContinuousDelivery** mode to ensure stable version numbers on the main branch.

### ContinuousDeployment vs ContinuousDelivery

| Aspect | ContinuousDeployment | ContinuousDelivery |
|--------|---------------------|-------------------|
| Main branch versions | `0.2.0-ci.110` (pre-release) | `0.2.0` (stable) |
| Automatic tagging | Adds CI/build tags | Only on release branches |
| NuGet publish | Pre-release packages | Stable packages |
| Best for | Every commit deploys | Controlled releases |

## Current Behavior

### Before (ContinuousDeployment)
```
Commit: "fix: nuget README.md"
Version: 0.2.0-ci0110
NuGet: Pre-release package
```

### After (ContinuousDelivery)
```
Commit: "fix: nuget README.md"
Version: 0.2.0
NuGet: Stable package
```

## How Versions Are Calculated

### Starting Point
- Last Git tag: `v0.1.1`
- Current commit: `c5ca091`

### Version Calculation Flow

1. **Find base version** from Git tags ? `0.1.1`
2. **Check commit messages** since last tag
3. **Count commits** with version bump indicators:
   - Found commits with `fix:` prefix ? Patch increment
   - Result: `0.1.1` ? `0.1.2`

### With Your Last Commit

Your commit message was:
```
fix: nuget README.md
```

This triggers a **patch increment** because:
- Message starts with `fix:` prefix
- Matches pattern: `^(fix|patch):`
- Action: Increment patch version

**Expected result**: `0.1.1` ? `0.1.2`

## Why It Showed 0.2.0-ci0110

The previous log showed `0.2.0-ci0110` because:

1. **ContinuousDeployment mode** was active
2. GitVersion counted **110 commits** since initial commit
3. It found commits that triggered **minor version bumps**
4. Added `-ci.110` tag because it's not a release branch

## Next Steps

### To Get Version 0.1.2 (Patch)
```bash
git commit -m "fix: Update README emojis"
git push origin main
```

### To Get Version 0.2.0 (Minor)
```bash
git commit -m "feature: Add new user search functionality"
git push origin main
```

### To Get Version 1.0.0 (Major)
```bash
git commit -m "breaking: Redesign authentication API"
git push origin main
```

## Testing Locally

```bash
# Install GitVersion globally
dotnet tool install --global GitVersion.Tool

# Check what version would be generated
dotnet-gitversion

# See specific version component
dotnet-gitversion /showvariable NuGetVersionV2
```

## Important Notes

1. **Versions are cumulative**: Each commit builds on the previous version
2. **Commit messages matter**: Use correct prefixes to control versioning
3. **Git tags are reference points**: GitVersion uses tags to find the base version
4. **Multiple commits**: If you have multiple commits, the highest version bump wins

## Example Scenarios

### Scenario 1: Bug Fix Release
```bash
# Current version: 0.1.1
git commit -m "fix: Correct user validation"
# New version: 0.1.2
```

### Scenario 2: Feature Release
```bash
# Current version: 0.1.2
git commit -m "feature: Add role-based permissions"
# New version: 0.2.0
```

### Scenario 3: Breaking Change
```bash
# Current version: 0.2.0
git commit -m "breaking: Remove legacy API endpoints"
# New version: 1.0.0
```

### Scenario 4: Multiple Commits
```bash
# Current version: 1.0.0
git commit -m "fix: Bug fix A"
git commit -m "feature: New feature B"
git commit -m "fix: Bug fix C"
# New version: 1.1.0 (highest is 'feature' = minor)
```

## Workflow Integration

The GitHub Actions workflow will:

1. ? Checkout code with full Git history
2. ? Install GitVersion
3. ? Calculate version from Git history and commits
4. ? Update `.csproj` files with calculated version
5. ? Build and pack NuGet packages
6. ? Publish to NuGet.org
7. ? Create Git tag (e.g., `v0.1.2`)

## Troubleshooting

### Issue: Version Not Incrementing
**Solution**: Check commit message format
```bash
# Wrong (no prefix)
git commit -m "Updated README"

# Correct (with prefix)
git commit -m "fix: Update README"
```

### Issue: Wrong Version Calculated
**Solution**: Check Git history
```bash
# See recent commits
git log --oneline -10

# See commits since last tag
git log v0.1.1..HEAD --oneline
```

### Issue: Want Specific Version
**Solution**: Create a Git tag
```bash
# Set exact version
git tag v1.0.0
git push origin v1.0.0
```

## References

- [GitVersion Documentation](https://gitversion.net/docs/)
- [GitVersion Modes](https://gitversion.net/docs/reference/modes/)
- [Semantic Versioning](https://semver.org/)
