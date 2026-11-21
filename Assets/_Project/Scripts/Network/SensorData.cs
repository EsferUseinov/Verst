using System;
using System.Collections.Generic;

[Serializable]
public class MachineReading
{
    public string id;
    public string name;
    public float temperature;
    public int rpm;
    public float load;
    public string status;
}

[Serializable]
public class SensorSnapshot
{
    public string updatedAt;
    public List<MachineReading> machines = new List<MachineReading>();
}
