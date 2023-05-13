namespace SmartHomeServer.Models
{
  public class DeviceHistory
  {
    public string DeviceId { get; set; }
    public string Name { get; set; }
    public DeviceType Type { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public TimeSpan OnTime { get; set; }
    public double Usage { get; set; }
    public bool IsOn { get; set; }

    public DateTime ModDate { get; set; }
    public int Wattage { get; set; }

  }
}
