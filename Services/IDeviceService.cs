using SmartHomeServer.Models;

namespace SmartHomeServer.Services
{
    public interface IDeviceService
    {
      Task<List<DeviceHistory>> GetDeviceDetails(DateTime fromDate, DateTime toDate);
      Task<List<LightDevice>> GetCurrentLights();
      Task<List<CameraDevice>> GetCurrentCameras();
      Task<List<ACDevice>> GetCurrentACs();
      Task<List<AlarmDevice>> GetCurrentAlarms();
  }
}
