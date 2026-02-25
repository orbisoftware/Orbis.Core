using Microsoft.AspNetCore.Identity;
using Orbis.Core.Abstractions;

namespace Orbis.Core.Models;


/// <summary>
/// Represents an application user extending the default Identity user.
/// </summary>
/// <remarks>
/// IdentityUser provides properties like UserName, Email, PasswordHash, PhoneNumber, 2FA, etc., which can be used for authentication and user management.
/// </remarks>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the role of the user. This is a required property that indicates the user's role within the application (e.g., Owner Admin, User).
    /// </summary>
    public required UserRole Role { get; set; }

    /// <summary>
    /// Returns a string representation of the application user including their username, email, and role.
    /// </summary>
    /// <returns>A formatted string containing the user's username, email, and role.</returns>
    public override string ToString()
    {
        return $"{UserName} ({Email}) - Role: {Role}";
    }
}
