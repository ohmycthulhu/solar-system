using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SatelliteInfo{
    public string Name;
    public int ID;
    public double Period;
    public SatelliteController Controller;
 
    public bool IsActive {
        get { return Controller.Active; }
    }

    public Transform Transform {
        get { return Controller.transform; }
    }
}
