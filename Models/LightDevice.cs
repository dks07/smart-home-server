﻿namespace SmartHomeServer.Models;

public class LightDevice : Device
{
    public string Color { get; set; }
    public int Brightness { get; set; }
}