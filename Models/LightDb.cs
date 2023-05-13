namespace SmartHomeServer.Models;

public class LightDb : DeviceDb, IEquatable<LightDb>
{
  public string Color { get; set; }
  public int Brightness { get; set; }

  public bool Equals(LightDb? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return base.Equals(other) && Color == other.Color && Brightness == other.Brightness;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((LightDb) obj);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(base.GetHashCode(), Color, Brightness);
  }
}