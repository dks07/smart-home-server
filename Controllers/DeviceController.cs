using Microsoft.AspNetCore.Mvc;
using SmartHomeServer.Services;

namespace SmartHomeServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DeviceController : ControllerBase
  {
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
      _deviceService = deviceService;
    }

    [HttpGet("camera")]
    public async Task<IActionResult> GetCurrentCameras()
    {
      var details = await _deviceService.GetCurrentCameras();
      return Ok(details);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetDeviceDetails(DateTime fromDate, DateTime toDate)
    {
      var details = await _deviceService.GetDeviceDetails(fromDate, toDate);
      return Ok(details);
    }


    [HttpGet("ac")]
    public async Task<IActionResult> GetCurrentACs()
    {
      var details = await _deviceService.GetCurrentACs();
      return Ok(details);
    }

    [HttpGet("alarm")]
    public async Task<IActionResult> GetCurrentAlarms()
    {
      var details = await _deviceService.GetCurrentAlarms();
      return Ok(details);
    }

    [HttpGet("light")]
    public async Task<IActionResult> GetCurrentLights()
    {
      var details = await _deviceService.GetCurrentLights();
      return Ok(details);
    }
  }
}
