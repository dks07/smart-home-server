using MongoDB.Driver;
using SmartHomeServer.Models;
using SmartHomeServer.Settings;

namespace SmartHomeServer.Services
{
  public class DeviceService : IDeviceService
  {
    private readonly IMongoCollection<CameraDb> _cameraDevices;
    private readonly IMongoCollection<ACDb> _acDevices;
    private readonly IMongoCollection<AlarmDb> _alarmDevices;
    private readonly IMongoCollection<LightDb> _lightDevices;
    public DeviceService(IMongoClient client, IDatabaseSettings settings)
    {
      var database = client.GetDatabase(settings.DatabaseName);
      _cameraDevices = database.GetCollection<CameraDb>("CameraDevices");
      _acDevices = database.GetCollection<ACDb>("ACDevices");
      _alarmDevices = database.GetCollection<AlarmDb>("AlarmDevices");
      _lightDevices = database.GetCollection<LightDb>("LightDevices");
    }
    public async Task<List<DeviceHistory>> GetDeviceDetails(DateTime fromDate, DateTime toDate)
    {
      var cameraSort = Builders<CameraDb>.Sort.Ascending("ModDate");
      var acSort = Builders<ACDb>.Sort.Ascending("ModDate");
      var alarmSort = Builders<AlarmDb>.Sort.Ascending("ModDate");
      var lightSort = Builders<LightDb>.Sort.Ascending("ModDate");
      var cameras = await _cameraDevices.Find(x => x.ModDate >= fromDate.ToUniversalTime() && x.ModDate <= toDate.ToUniversalTime()).Sort(cameraSort).ToListAsync();
      var acs = await _acDevices.Find(x => x.ModDate >= fromDate.ToUniversalTime() && x.ModDate <= toDate.ToUniversalTime()).Sort(acSort).ToListAsync();
      var alarms = await _alarmDevices.Find(x => x.ModDate >= fromDate.ToUniversalTime() && x.ModDate <= toDate.ToUniversalTime()).Sort(alarmSort).ToListAsync();
      var lights = await _lightDevices.Find(x => x.ModDate >= fromDate.ToUniversalTime() && x.ModDate <= toDate.ToUniversalTime()).Sort(lightSort).ToListAsync();
      List<DeviceHistory> deviceHistories = new List<DeviceHistory>();
      foreach (var camera in cameras)
      {
        var foundDevice = deviceHistories.FirstOrDefault(x => x.DeviceId == camera.DeviceId);
        if (foundDevice == null)
        {
          deviceHistories.Add(new DeviceHistory
          {
            DeviceId = camera.DeviceId,
            Description = camera.Description,
            Location = camera.Location,
            Name = camera.Name,
            Type = camera.Type,
            OnTime = TimeSpan.Zero,
            Usage = 0,
            IsOn = camera.IsOn,
            ModDate = camera.ModDate,
            Wattage = camera.Wattage
          });
        }
        else
        {
          if (foundDevice.IsOn)
          {
            var onTime = camera.ModDate.Subtract(foundDevice.ModDate);
            foundDevice.OnTime = foundDevice.OnTime.Add(onTime);
            foundDevice.Usage += (camera.Wattage * onTime.TotalSeconds)/3600000;
          }
          foundDevice.Description = camera.Description;
          foundDevice.Location = camera.Location;
          foundDevice.Name = camera.Name;
          foundDevice.Type = camera.Type;
          foundDevice.IsOn = camera.IsOn;
          foundDevice.ModDate = camera.ModDate;
          foundDevice.Wattage = camera.Wattage;
        }
      }
      foreach (var ac in acs)
      {
        var foundDevice = deviceHistories.FirstOrDefault(x => x.DeviceId == ac.DeviceId);
        if (foundDevice == null)
        {
          deviceHistories.Add(new DeviceHistory
          {
            DeviceId = ac.DeviceId,
            Description = ac.Description,
            Location = ac.Location,
            Name = ac.Name,
            Type = ac.Type,
            OnTime = TimeSpan.Zero,
            Usage = 0,
            IsOn = ac.IsOn,
            ModDate = ac.ModDate,
            Wattage = ac.Wattage
          });
        }
        else
        {
          if (foundDevice.IsOn)
          {
            var onTime = ac.ModDate.Subtract(foundDevice.ModDate);
            foundDevice.OnTime = foundDevice.OnTime.Add(onTime);
            foundDevice.Usage += (ac.Wattage * onTime.TotalSeconds) / 3600000;
          }
          foundDevice.Description = ac.Description;
          foundDevice.Location = ac.Location;
          foundDevice.Name = ac.Name;
          foundDevice.Type = ac.Type;
          foundDevice.IsOn = ac.IsOn;
          foundDevice.ModDate = ac.ModDate;
          foundDevice.Wattage = ac.Wattage;
        }
      }
      foreach (var alarm in alarms)
      {
        var foundDevice = deviceHistories.FirstOrDefault(x => x.DeviceId == alarm.DeviceId);
        if (foundDevice == null)
        {
          deviceHistories.Add(new DeviceHistory
          {
            DeviceId = alarm.DeviceId,
            Description = alarm.Description,
            Location = alarm.Location,
            Name = alarm.Name,
            Type = alarm.Type,
            OnTime = TimeSpan.Zero,
            Usage = 0,
            IsOn = alarm.IsOn,
            ModDate = alarm.ModDate,
            Wattage = alarm.Wattage
          });
        }
        else
        {
          if (foundDevice.IsOn)
          {
            var onTime = alarm.ModDate.Subtract(foundDevice.ModDate);
            foundDevice.OnTime = foundDevice.OnTime.Add(onTime);
            foundDevice.Usage += (alarm.Wattage * onTime.TotalSeconds) / 3600000;
          }
          foundDevice.Description = alarm.Description;
          foundDevice.Location = alarm.Location;
          foundDevice.Name = alarm.Name;
          foundDevice.Type = alarm.Type;
          foundDevice.IsOn = alarm.IsOn;
          foundDevice.ModDate = alarm.ModDate;
          foundDevice.Wattage = alarm.Wattage;
        }
      }
      foreach (var light in lights)
      {
        var foundDevice = deviceHistories.FirstOrDefault(x => x.DeviceId == light.DeviceId);
        if (foundDevice == null)
        {
          deviceHistories.Add(new DeviceHistory
          {
            DeviceId = light.DeviceId,
            Description = light.Description,
            Location = light.Location,
            Name = light.Name,
            Type = light.Type,
            OnTime = TimeSpan.Zero,
            Usage = 0,
            IsOn = light.IsOn,
            ModDate = light.ModDate,
            Wattage = light.Wattage
          });
        }
        else
        {
          if (foundDevice.IsOn)
          {
            var onTime = light.ModDate.Subtract(foundDevice.ModDate);
            foundDevice.OnTime = foundDevice.OnTime.Add(onTime);
            foundDevice.Usage += (light.Wattage * onTime.TotalSeconds) / 3600000;
          }
          foundDevice.Description = light.Description;
          foundDevice.Location = light.Location;
          foundDevice.Name = light.Name;
          foundDevice.Type = light.Type;
          foundDevice.IsOn = light.IsOn;
          foundDevice.ModDate = light.ModDate;
          foundDevice.Wattage = light.Wattage;
        }
      }

      foreach (var deviceHistory in deviceHistories)
      {
        if (deviceHistory.IsOn)
        {
          var onTime = toDate > DateTime.Now ? DateTime.Now.Subtract(deviceHistory.ModDate.ToLocalTime()) : toDate.Subtract(deviceHistory.ModDate.ToLocalTime());

          deviceHistory.OnTime = deviceHistory.OnTime.Add(onTime);
          deviceHistory.Usage += (deviceHistory.Wattage * onTime.TotalSeconds) / 3600000;
        }
      }
      return deviceHistories;


      
    }

    public async Task<List<LightDevice>> GetCurrentLights()
    {
      var lightSort = Builders<LightDb>.Sort.Descending("ModDate");
      var lightsDevice = await _lightDevices.Distinct<string>("DeviceId", FilterDefinition<LightDb>.Empty).ToListAsync();
      var lights = new List<LightDevice>();
      foreach (var lightDevice in lightsDevice)
      {
        var lightDb = await _lightDevices.Find(x => x.DeviceId == lightDevice).Sort(lightSort).Limit(1).FirstOrDefaultAsync();
        if (lightDb == null) return lights;
        lights.Add(new LightDevice
        {
          DeviceId = lightDb.DeviceId,
          Description = lightDb.Description,
          Location = lightDb.Location,
          Name = lightDb.Name,
          Type = lightDb.Type,
          IsOn = lightDb.IsOn,
          Wattage = lightDb.Wattage,
          Brightness = lightDb.Brightness,
          Color = lightDb.Color,
          Status = lightDb.Status
        });
      }

      return lights;

    }
    public async Task<List<CameraDevice>> GetCurrentCameras()
    {
      var cameraSort = Builders<CameraDb>.Sort.Descending("ModDate");
      var camerasDevice = await _cameraDevices.Distinct<string>("DeviceId", FilterDefinition<CameraDb>.Empty).ToListAsync();
      var cameras = new List<CameraDevice>();
      foreach (var cameraDevice in camerasDevice)
      {
        var cameraDb = await _cameraDevices.Find(x => x.DeviceId == cameraDevice).Sort(cameraSort).Limit(1).FirstOrDefaultAsync();
        if (cameraDb == null) return cameras;
        cameras.Add(new CameraDevice
        {
          DeviceId = cameraDb.DeviceId,
          Description = cameraDb.Description,
          Location = cameraDb.Location,
          Name = cameraDb.Name,
          Type = cameraDb.Type,
          IsOn = cameraDb.IsOn,
          Wattage = cameraDb.Wattage,
          Status = cameraDb.Status,
          IsStreaming = cameraDb.IsStreaming,
          IsRecording = cameraDb.IsRecording
        });
      }

      return cameras;
    }
    public async Task<List<ACDevice>> GetCurrentACs()
    {
      var acSort = Builders<ACDb>.Sort.Descending("ModDate");
      var acsDevice = await _acDevices.Distinct<string>("DeviceId", FilterDefinition<ACDb>.Empty).ToListAsync();
      var aCs = new List<ACDevice>();
      foreach (var acDevice in acsDevice)
      {
        var acDb = await _acDevices.Find(x => x.DeviceId == acDevice).Sort(acSort).Limit(1).FirstOrDefaultAsync();
        if (acDb == null) continue;
        aCs.Add(new ACDevice
        {
          DeviceId = acDb.DeviceId,
          Description = acDb.Description,
          Location = acDb.Location,
          Name = acDb.Name,
          Type = acDb.Type,
          IsOn = acDb.IsOn,
          Wattage = acDb.Wattage,
          Status = acDb.Status,
          Humidity = acDb.Humidity,
          Temperature = acDb.Temperature
        });
      }

      return aCs;
    }
    public async Task<List<AlarmDevice>> GetCurrentAlarms()
    {
      var alarmSort = Builders<AlarmDb>.Sort.Descending("ModDate");
      var alarmsDevice = await _alarmDevices.Distinct<string>("DeviceId", FilterDefinition<AlarmDb>.Empty).ToListAsync();
      var alarms = new List<AlarmDevice>();
      foreach (var alarmDevice in alarmsDevice)
      {
        var alarmDb = await _alarmDevices.Find(x => x.DeviceId == alarmDevice).Sort(alarmSort).Limit(1).FirstOrDefaultAsync(); 
        if (alarmDb == null) continue;
        alarms.Add(new AlarmDevice
        {
          DeviceId = alarmDb.DeviceId,
          Description = alarmDb.Description,
          Location = alarmDb.Location,
          Name = alarmDb.Name,
          Type = alarmDb.Type,
          IsOn = alarmDb.IsOn,
          Wattage = alarmDb.Wattage,
          Status = alarmDb.Status,
          IsTriggered = alarmDb.IsTriggered,
          SmokeLevel = alarmDb.SmokeLevel
        });
      }
      return alarms;
    }
  }
}
