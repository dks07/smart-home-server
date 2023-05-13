using MongoDB.Bson;
using MongoDB.Driver;
using SmartHomeServer.Models;
using SmartHomeServer.Settings;

namespace SmartHomeServer.Services
{
  public class ACDeviceService
  {
    private readonly IMongoCollection<ACDb> _acDevices;
    public ACDeviceService(IMongoClient client, IDatabaseSettings settings)
    {
      var database = client.GetDatabase(settings.DatabaseName);
      _acDevices = database.GetCollection<ACDb>("ACDevices");
    }
    public async Task Process(Device device)
    {
      ACDevice? acDevice = device as ACDevice;
      if (acDevice == null) return;
      var sort = Builders<ACDb>.Sort.Descending("ModDate");
      ACDb lastResult = await _acDevices.Find(x => x.DeviceId == device.DeviceId).Sort(sort).Limit(1).FirstOrDefaultAsync();
      ACDb acDb = new ACDb
      {
        Type = acDevice.Type,
        Description = acDevice.Description,
        DeviceId = acDevice.DeviceId,
        Id = ObjectId.GenerateNewId().ToString(),
        IsOn = acDevice.IsOn,
        Location = acDevice.Location,
        ModDate = DateTime.Now,
        Name = acDevice.Name,
        Status = acDevice.Status,
        Humidity = acDevice.Humidity,
        Temperature = acDevice.Temperature,
        Wattage = acDevice.Wattage
      };
      if (lastResult == null || !lastResult.Equals(acDb))
      {
        await _acDevices.InsertOneAsync(acDb);
      }
    }
  }
}
