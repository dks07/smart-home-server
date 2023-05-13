using MongoDB.Bson;
using MongoDB.Driver;
using SmartHomeServer.Models;
using SmartHomeServer.Settings;

namespace SmartHomeServer.Services
{
  public class AlarmDeviceService
  {
    private readonly IMongoCollection<AlarmDb> _alarmDevices;
    public AlarmDeviceService(IMongoClient client, IDatabaseSettings settings)
    {
      var database = client.GetDatabase(settings.DatabaseName);
      _alarmDevices = database.GetCollection<AlarmDb>("AlarmDevices");
    }

    public async Task Process(Device device)
    {
      AlarmDevice? alarmDevice = device as AlarmDevice;
      if(alarmDevice == null) return;
      var sort = Builders<AlarmDb>.Sort.Descending("ModDate");
      AlarmDb lastResult = await _alarmDevices.Find(x => x.DeviceId == device.DeviceId).Sort(sort).Limit(1).FirstOrDefaultAsync();
      AlarmDb alarmDb = new AlarmDb
      {
        Type = alarmDevice.Type,
        Description = alarmDevice.Description,
        DeviceId = alarmDevice.DeviceId,
        Id = ObjectId.GenerateNewId().ToString(),
        IsOn = alarmDevice.IsOn,
        IsTriggered = alarmDevice.IsTriggered,
        Location = alarmDevice.Location,
        ModDate = DateTime.Now,
        Name = alarmDevice.Name,
        SmokeLevel = alarmDevice.SmokeLevel,
        Status = alarmDevice.Status,
        Wattage = alarmDevice.Wattage
      };
      if (lastResult == null || !lastResult.Equals(alarmDb))
      {
        await _alarmDevices.InsertOneAsync(alarmDb);
      }
    }
  }
}
