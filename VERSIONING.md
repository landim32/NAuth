# GitVersion Configuration

This project uses [GitVersion](https://gitversion.net/) for automatic semantic versioning based on Git history and commit messages.

## Version Strategy

The project uses **ContinuousDelivery** mode, which means:
- **Main branch**: Generates stable versions (e.g., `0.2.0`, `1.0.0`)
- **Develop branch**: Minor increments with alpha tag (e.g., `0.2.0-alpha.1`)
- **Feature branches**: Minor increments with alpha tag (e.g., `0.2.0-alpha.2`)
- **Release branches**: Patch increments with beta tag (e.g., `0.2.0-beta.1`)
- **Hotfix branches**: Patch increments (stable versions)

## Branch Configuration

- **Main branch**: Stable releases only - Patch increments by default
- **Develop branch**: Development work - Minor increments with alpha tag
- **Feature branches**: New features - Minor increments with alpha tag
- **Release branches**: Release candidates - Patch increments with beta tag
- **Hotfix branches**: Emergency fixes - Patch increments

## Controlling Version Increments

You can control version increments using conventional commit message prefixes:

### Major Version Bump (Breaking Changes)
Use `major:` or `breaking:` prefix:

```bash
git commit -m "major: Redesign API interface"
git commit -m "breaking: Remove deprecated methods"
```

**Result**: `1.0.0` ? `2.0.0`

### Minor Version Bump (New Features)
Use `feature:` or `minor:` prefix:

```bash
git commit -m "feature: Add new authentication method"
git commit -m "minor: Add user search functionality"
```

**Result**: `1.0.0` ? `1.1.0`

### Patch Version Bump (Bug Fixes)
Use `fix:` or `patch:` prefix, or any other commit message:

```bash
git commit -m "fix: Fix user validation bug"
git commit -m "patch: Correct email validation"
git commit -m "Update dependencies"
```

**Result**: `1.0.0` ? `1.0.1`

### Skip Version Increment
Use `+semver: none` or `+semver: skip`:

```bash
git commit -m "Update documentation +semver: none"
git commit -m "Refactor code style +semver: skip"
```

**Result**: No version change

## Commit Message Format

### Recommended Format

```
<type>: <short description>

[optional body]

[optional footer]
```

### Examples

#### Major Changes (Breaking)
```bash
git commit -m "major: Redesign authentication API"
git commit -m "breaking: Remove support for .NET 6"
```

#### Minor Changes (Features)
```bash
git commit -m "feature: Add role-based access control"
git commit -m "minor: Add password strength validation"
```

#### Patch Changes (Fixes)
```bash
git commit -m "fix: Resolve null reference in user service"
git commit -m "patch: Correct JWT token expiration"
git commit -m "Improve error handling"
```

## How It Works

1. **GitVersion analyzes** your Git history and branch structure
2. **Calculates the next version** based on:
   - Current branch
   - Commit messages with type prefixes
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

# Show detailed information
dotnet-gitversion /showvariable FullSemVer
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

## Complete Examples

### Starting with version 0.1.1 on main branch:

```bash
# Regular commit (patch increment)
git commit -m "Update user validation logic"
# Result: 0.1.2

# Bug fix (patch increment)
git commit -m "fix: Fix authentication bug"
# Result: 0.1.3

# Feature commit (minor increment)
git commit -m "feature: Add role management"
# Result: 0.2.0

# Another feature (minor increment)
git commit -m "minor: Add user search"
# Result: 0.3.0

# Breaking change (major increment)
git commit -m "major: Redesign API interface"
# Result: 1.0.0

# Another breaking change (major increment)
git commit -m "breaking: Remove legacy authentication"
# Result: 2.0.0

# Documentation update (no increment)
git commit -m "Update README +semver: none"
# Result: 2.0.0 (no change)
```

## Conventional Commits Compatibility

This configuration is compatible with [Conventional Commits](https://www.conventionalcommits.org/) specification:

- `major:` ? Breaking change
- `breaking:` ? Breaking change
- `feature:` ? New feature
- `minor:` ? New feature
- `fix:` ? Bug fix
- `patch:` ? Bug fix
- Other types (`docs:`, `chore:`, `refactor:`, etc.) ? Patch increment

## Best Practices

1. **Be descriptive**: Write clear commit messages
2. **Use prefixes**: Always use appropriate type prefixes
3. **One purpose**: One commit should have one purpose
4. **Test locally**: Use `dotnet-gitversion` to verify version calculation
5. **Review history**: Check Git history before pushing

## References

- [GitVersion Documentation](https://gitversion.net/docs/)
- [Semantic Versioning](https://semver.org/)
- [Conventional Commits](https://www.conventionalcommits.org/)
