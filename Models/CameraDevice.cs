namespace SmartHomeServer.Models;

public class CameraDevice : Device
{
    public bool IsRecording { get; set; }
    public bool IsStreaming { get; set; }
}