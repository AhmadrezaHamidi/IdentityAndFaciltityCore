namespace FacilityService.Domain.Entity.Common;

/// <summary>
/// همزمانی خوش‌بینانه: Version با هر تغییر یک واحد زیاد می‌شود و EF آن را Concurrency Token می‌گیرد.
/// RowVersion در API همین عدد به‌صورت رشته است (روی SQL Server و SQLite یکسان کار می‌کند).
/// </summary>
public abstract class VersionedEntity
{
    public int Version { get; protected set; } = 1;

    public string RowVersion => Version.ToString();

    protected void Bump() => Version++;

    public void EnsureRowVersion(string? rowVersion)
    {
        if (rowVersion != RowVersion) throw FacilityDomainException.Conflict();
    }
}
