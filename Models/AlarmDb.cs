namespace SmartHomeServer.Models;

public class AlarmDb : DeviceDb, IEquatable<AlarmDb>
{
  public bool IsTriggered { get; set; }
  public float SmokeLevel { get; set; }

  public bool Equals(AlarmDb? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return base.Equals(other) && IsTriggered == other.IsTriggered && SmokeLevel.Equals(other.SmokeLevel);
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((AlarmDb) obj);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(base.GetHashCode(), IsTriggered, SmokeLevel);
  }
}