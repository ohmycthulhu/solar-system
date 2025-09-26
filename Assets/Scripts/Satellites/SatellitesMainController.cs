using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class SatellitesMainController{
    [System.Serializable]
    private class SatListFormat {
        public SatInfoFormat[] sat_list;
    }
    [System.Serializable]
    private class SatInfoFormat {
        public int id;
        public string name;
        public double period;
    }
    [System.Serializable]
    public class DataFormatUnit {
        public double perigee;
        public double period;
        public double eccentricity;
        public int id;
        public string name;
        public string int_design;
        public string time;
        public double y;
        public double x;
        public double z;
        public double apogee;
        public double inclination;
    }
    [System.Serializable]
    public class DataFormatMain {
        public DataFormatUnit[] data_list;
    }
    static bool _initialized = false;
    static Dictionary<string,string[]> _availableURLs = new Dictionary<string, string[]>();
    static string[] _urlsSatInfoAll = {
            "http://127.0.0.1:5000/api",
            "http://127.0.0.1:5000/api",
    };

    static string[] _urlsSatListAll = {
        "http://127.0.0.1:5000/api",
        "http://127.0.0.1:5000/api",
    };
    private static SatelliteInfo[] _satellitesAll;
    private static Dictionary<string, SatellitePosition[]> _downloadResults;
    public static string[] AllSatellites {
        get {
            if (_satellitesAll == null) _satellitesAll = new SatelliteInfo[0];
            return _satellitesAll.Select(x => x.Name).ToArray();
        }
    }
    private static Dictionary<string,string[]> _availableURLsResult;
    private static bool GetURLs(string fileName, out string[][] result) {
        result = default(string[][]);
        try {
            result = File.ReadAllLines(fileName)
                .Select(x => x.Split(' '))
                .ToArray();
        }
        catch {
            return false;
        }
        return true;
    }

    // TODO: Remake method to return the result
    private static IEnumerator GetAvailableURLs(string[] urlsList,string[] urlsInfo) {
        // TODO: Introduce a data type with info and list properties
        Dictionary<string, List<string>> availableURLs = new Dictionary<string, List<string>>();
        availableURLs["info"] = new List<string>();
        availableURLs["list"] = new List<string>();
        List<string> availables = new List<string>();
        for(int i = 0; i < urlsList.Length;i++) {
            string url = urlsList[i];
            // TODO: Do I need to load a list of satellites here?!
            using(WWW www = new WWW(url)) {
                yield return www;
                if(www.error == null) {
                    availableURLs["list"].Add(url);
                    if (urlsInfo.Length > i)
                        availableURLs["info"].Add(urlsInfo[i]);
                }
            }
        }
        _availableURLsResult = availableURLs.ToDictionary(x=>x.Key,y=>y.Value.ToArray());
    }
    public static IEnumerator Initialize() {
        _initialized = true;
        _downloadResults = new Dictionary<string, SatellitePosition[]>();
        string[][] urls;
        if (GetURLs("config/urls.txt",out urls)) {
            _urlsSatListAll = urls[0];
            _urlsSatInfoAll= urls[1];
        }
        yield return GetAvailableURLs(_urlsSatListAll,_urlsSatInfoAll);
        _availableURLs = _availableURLsResult;
        yield return GetSatellitesList();
    } 
    public static IEnumerator GetSatellitesList() {
        // TODO: Rework the method
        List<SatelliteInfo> satellites = new List<SatelliteInfo>();
        foreach (string url in _availableURLs["list"]) {
            using (WWW www = new WWW(url)) {
                yield return www;
                satellites.AddRange(JsonUtility.FromJson<SatListFormat>(www.text)
                    .sat_list
                    .Select(x => new SatelliteInfo() {
                        Name = x.name,
                        ID = x.id,
                        Period = x.period / 96400
                    }
                    ));
            }
        }
        _satellitesAll = satellites
            .GroupBy(x=>x.Name)
            .SelectMany(x=>x)
            .ToArray();
    }


    private static IEnumerator DownloadData(int id, decimal start_time, decimal end_time, int period,string saveAs) {
        foreach (string url in _availableURLs["info"]) {
            string request = string.Format(url + "?id={0}&start_time={1}&end_time={2}&period={3}", 
                id, System.Math.Floor(start_time), System.Math.Ceiling(end_time), period);
            using (WWW www = new WWW(request)) {
                yield return www;
                if (www.error == null) {
                    SatellitePosition[] si = JsonUtility.FromJson<DataFormatMain>(www.text)
                        .data_list
                        .Select(u => new SatellitePosition() {
                            X = u.x,
                            Y = u.z,
                            Z = u.y,
                            Name = u.name,
                            Time = TimeSystem.ToUniversalUnit(
                                System.DateTime.Parse(u.time).ToUniversalTime()
                                )
                        })
                        .ToArray();

                    _downloadResults[saveAs] = si;
                    break;
                }
                else {
                    Debug.Log(www.error);
                }
            }
        }
    }

    public static IEnumerator PrepareSatelliteInfo(string name, decimal start_time, decimal end_time, int period) {
        if (_satellitesAll != null) {
            var similarSatellites = _satellitesAll.Where(x => x.Name == name);
            if (similarSatellites.Count() != 0) {
                yield return DownloadData(similarSatellites.First().ID, start_time, end_time, period, name);
            }
        }
    }

    public static SatellitePosition[] GetDownloadsResult(string name) {
        return _downloadResults.ContainsKey(name) ? _downloadResults[name] : new SatellitePosition[0];
    }
    
    public static SatelliteInfo GetSatelliteInfo(string name) {
        // TODO: Replace with Find + Elvis
        return _satellitesAll.Any(x => x.Name == name) ? _satellitesAll.Where(x => x.Name == name).First() : default(SatelliteInfo);
    }

    public static bool DoesSatelliteExists(string name) {
        return _satellitesAll.Any(x => x.Name == name);
    }
}
