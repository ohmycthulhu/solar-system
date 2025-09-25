using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
/*
public static class DataLoader{
    static Unit[] cash = null;
    public static SatelliteInfo[] GetSatelliteInfo(string name = "",decimal time = 0)
    {
        if(cash == null)
        {
            var jsonData = File.ReadAllText("Positions/Satellite.txt");
            cash = Unserialize(jsonData);
        }
        return cash.Select((u) => 
        {
            return new SatelliteInfo()
            { X = u.x, Y = u.y, Z = u.z, Name = u.name, Time = TimeSystem.ToUniversalUnit(System.DateTime.Parse(u.time))};
        }).ToArray();
    }
	public static Unit[] Unserialize(string jsonStr)
    {
        try
        {
            Main data = JsonUtility.FromJson<Main>(jsonStr);
            return data.data_list;
        }
        catch
        {
            return null;
        }
    }
}*/
