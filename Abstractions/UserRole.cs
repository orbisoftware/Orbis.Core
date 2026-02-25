namespace Orbis.Core.Abstractions;

/// <summary>
/// Defines the roles available for users in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Owner role with full system access.
    /// </summary>
    Owner = 2,
    /// <summary>
    /// Admin role with administrative privileges.
    /// </summary>
    Admin = 1,
    /// <summary>
    /// User role with standard privileges.
    /// </summary>
    User = 0
}