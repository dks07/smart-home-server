namespace SmartHomeServer.Models;

public class ACDb : DeviceDb, IEquatable<ACDb>
{
  public int Temperature { get; set; }
  public int Humidity { get; set; }

  public bool Equals(ACDb? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return base.Equals(other) && Temperature == other.Temperature && Humidity == other.Humidity;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((ACDb) obj);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(base.GetHashCode(), Temperature, Humidity);
  }
}