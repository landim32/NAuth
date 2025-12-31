# GitVersion Configuration

This project uses [GitVersion](https://gitversion.net/) for automatic semantic versioning based on Git history and commit messages.

## Version Strategy

- **Main branch**: Patch increments by default
- **Develop branch**: Minor increments with alpha tag
- **Feature branches**: Minor increments with alpha tag
- **Release branches**: Patch increments with beta tag
- **Hotfix branches**: Patch increments

## Controlling Version Increments

You can control version increments using special markers in your commit messages:

### Major Version Bump (Breaking Changes)
```
+semver: major
+semver: breaking
```

Example:
```bash
git commit -m "Refactor API interface +semver: breaking"
```

### Minor Version Bump (New Features)
```
+semver: minor
+semver: feature
```

Example:
```bash
git commit -m "Add new authentication method +semver: feature"
```

### Patch Version Bump (Bug Fixes)
```
+semver: patch
+semver: fix
```

Example:
```bash
git commit -m "Fix user validation bug +semver: patch"
```

### Skip Version Increment
```
+semver: none
+semver: skip
```

Example:
```bash
git commit -m "Update documentation +semver: none"
```

## How It Works

1. **GitVersion analyzes** your Git history and branch structure
2. **Calculates the next version** based on:
   - Current branch
   - Commit messages with semver markers
   - Previous tags
3. **Updates project files** with the calculated version
4. **Creates a Git tag** after successful NuGet publication

## Local Testing

To test version calculation locally:

```bash
# Install GitVersion tool
dotnet tool install --global GitVersion.Tool

# Calculate version
dotnet-gitversion
```

## CI/CD Integration

The GitHub Actions workflow automatically:
1. Installs GitVersion
2. Calculates the version based on Git history
3. Updates `.csproj` files with the new version
4. Builds and publishes NuGet packages
5. Creates a Git tag for the release

## Version Format

- **NuGet packages**: Uses `NuGetVersionV2` format (e.g., `1.2.3`, `1.2.3-alpha.1`)
- **Git tags**: Uses `v{Major}.{Minor}.{Patch}` format (e.g., `v1.2.3`)

## Examples

### Starting with version 0.1.1 on main branch:

```bash
# Regular commit (patch increment)
git commit -m "Fix authentication bug"
# Result: 0.1.2

# Feature commit (minor increment)
git commit -m "Add role management +semver: feature"
# Result: 0.2.0

# Breaking change commit (major increment)
git commit -m "Redesign API interface +semver: breaking"
# Result: 1.0.0

# Documentation update (no increment)
git commit -m "Update README +semver: none"
# Result: 1.0.0 (no change)
```

## References

- [GitVersion Documentation](https://gitversion.net/docs/)
- [Semantic Versioning](https://semver.org/)
