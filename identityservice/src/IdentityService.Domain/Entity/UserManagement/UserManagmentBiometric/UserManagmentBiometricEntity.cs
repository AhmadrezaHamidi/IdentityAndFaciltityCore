namespace IdentityService.Domain.Entity.UserManagement.UserManagmentBiometric;

public partial class UserManagmentBiometricEntity
{
    public int Id { get; set; }

    public int UserManagmentId { get; set; }

    public string BiometricToken { get; set; } = null!;

    public string BiometricKey { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public bool IsActive { get; set; }
}
