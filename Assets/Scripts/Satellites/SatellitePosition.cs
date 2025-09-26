using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SatellitePosition {
    public double X, Y, Z;
    public string Name;
    public decimal Time;
    public Vector3 Position {
        get { return new Vector3((float)X, (float)Y, (float)Z); }
        set { X = value.x;Y = value.y;Z = value.z; }
    }
}
