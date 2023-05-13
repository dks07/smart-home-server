using MongoDB.Bson;
using MongoDB.Driver;
using SmartHomeServer.Models;
using SmartHomeServer.Settings;

namespace SmartHomeServer.Services
{
  public class CameraDeviceService
  {
    private readonly IMongoCollection<CameraDb> _cameraDevices;
    public CameraDeviceService(IMongoClient client, IDatabaseSettings settings)
    {
      var database = client.GetDatabase(settings.DatabaseName);
      _cameraDevices = database.GetCollection<CameraDb>("CameraDevices");
    }
    public async Task Process(Device device)
    {
      CameraDevice? cameraDevice = device as CameraDevice;
      if (cameraDevice == null) return;
      var sort = Builders<CameraDb>.Sort.Descending("ModDate");
      CameraDb lastResult = await _cameraDevices.Find(x => x.DeviceId == device.DeviceId).Sort(sort).Limit(1).FirstOrDefaultAsync();
      CameraDb cameraDb = new CameraDb
      {
        Type = cameraDevice.Type,
        Description = cameraDevice.Description,
        DeviceId = cameraDevice.DeviceId,
        Id = ObjectId.GenerateNewId().ToString(),
        IsOn = cameraDevice.IsOn,
        Location = cameraDevice.Location,
        ModDate = DateTime.Now,
        Name = cameraDevice.Name,
        Status = cameraDevice.Status,
        IsRecording = cameraDevice.IsRecording,
        IsStreaming = cameraDevice.IsStreaming,
        Wattage = cameraDevice.Wattage
      };
      if (lastResult == null || !lastResult.Equals(cameraDb))
      {
        await _cameraDevices.InsertOneAsync(cameraDb);
      }
    }
  }
}
