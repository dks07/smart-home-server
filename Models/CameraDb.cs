namespace SmartHomeServer.Models;

public class CameraDb : DeviceDb, IEquatable<CameraDb>
{
  public bool IsRecording { get; set; }
  public bool IsStreaming { get; set; }

  public bool Equals(CameraDb? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return base.Equals(other) && IsRecording == other.IsRecording && IsStreaming == other.IsStreaming;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((CameraDb) obj);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(base.GetHashCode(), IsRecording, IsStreaming);
  }
}