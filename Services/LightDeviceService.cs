using MongoDB.Bson;
using MongoDB.Driver;
using SmartHomeServer.Models;
using SmartHomeServer.Settings;

namespace SmartHomeServer.Services
{
  public class LightDeviceService
  {
    private readonly IMongoCollection<LightDb> _lightDevices;
    public LightDeviceService(IMongoClient client, IDatabaseSettings settings)
    {
      var database = client.GetDatabase(settings.DatabaseName);
      _lightDevices = database.GetCollection<LightDb>("LightDevices");
    }
    public async Task Process(Device device)
    {
      LightDevice? lightDevice = device as LightDevice;
      if (lightDevice == null) return;
      var sort = Builders<LightDb>.Sort.Descending("ModDate");
      LightDb lastResult = await _lightDevices.Find(x => x.DeviceId == device.DeviceId).Sort(sort).Limit(1).FirstOrDefaultAsync();
      LightDb lightDb = new LightDb
      {
        Type = lightDevice.Type,
        Description = lightDevice.Description,
        DeviceId = lightDevice.DeviceId,
        Id = ObjectId.GenerateNewId().ToString(),
        IsOn = lightDevice.IsOn,
        Location = lightDevice.Location,
        ModDate = DateTime.Now,
        Name = lightDevice.Name,
        Status = lightDevice.Status,
        Brightness = lightDevice.Brightness,
        Color = lightDevice.Color,
        Wattage = lightDevice.Wattage
      };
      if (lastResult == null || !lastResult.Equals(lightDb))
      {
        await _lightDevices.InsertOneAsync(lightDb);
      }
    }
  }
}
