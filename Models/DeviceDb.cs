using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SmartHomeServer.Models;

public abstract class DeviceDb : IEquatable<DeviceDb>
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }
  public string DeviceId { get; set; }
  public string Name { get; set; }
  public DeviceType Type { get; set; }
  public string Location { get; set; }
  public string Description { get; set; }
  public DeviceStatus Status { get; set; }
  public bool IsOn { get; set; }
  public int Wattage { get; set; }

  public DateTime ModDate { get; set; }

  public bool Equals(DeviceDb? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return DeviceId == other.DeviceId && Name == other.Name && Type == other.Type && Location == other.Location && Description == other.Description && Status == other.Status && IsOn == other.IsOn && Wattage == other.Wattage;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((DeviceDb) obj);
  }

  public override int GetHashCode()
  {
    var hashCode = new HashCode();
    hashCode.Add(DeviceId);
    hashCode.Add(Name);
    hashCode.Add((int) Type);
    hashCode.Add(Location);
    hashCode.Add(Description);
    hashCode.Add((int) Status);
    hashCode.Add(IsOn);
    return hashCode.ToHashCode();
  }
}