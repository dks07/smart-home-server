﻿namespace SmartHomeServer.Models;

public class ACDevice : Device
{
    public int Temperature { get; set; }
    public int Humidity { get; set; }
}