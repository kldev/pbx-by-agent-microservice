namespace App.Bps.Enum;

/// <summary>
/// System permissions for fine-grained access control.
/// These permissions are assigned to users independently of roles.
/// NOTE: This is just an idea for now - not used anywhere in the API yet.
/// </summary>
public enum AppPermission
{
    /// <summary>
    /// Access to invoices and bank operations
    /// </summary>
    InvoicesBank = 1,

    /// <summary>
    /// Access to sensitive data.
    /// </summary>
    SensitiveData = 2
}
