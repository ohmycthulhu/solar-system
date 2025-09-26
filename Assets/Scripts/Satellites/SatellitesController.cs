using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class SatellitesController : MonoBehaviour
{
    private List<SatelliteInfo> activeSatellites;
    private PlanetSelector _planetSelector;
    public SearchField searchField = null;
    [SerializeField]
    private bool _randomColor;
   
    public bool RandomColor {
        get {
            return _randomColor;
        }

        set {
            _randomColor = value;
        }
    }

    private List<string> availableURLs;
    public IEnumerator Start() {
        yield return SatellitesMainController.Initialize();
        string[] satellitesNames = SatellitesMainController.AllSatellites;
        activeSatellites = new List<SatelliteInfo>();
        if (searchField != null) {
            searchField.Items = satellitesNames;
        }

        searchField.OnSelect = delegate (string t) {
            this.AddSatellite(t);
        };
        _planetSelector = FindObjectOfType<PlanetSelector>();
    }
    public void Update() { }

    public void OnGUI() {
        if (!GUIController.Enable || activeSatellites == null) return;
        float height = 5;
        foreach (var sat in activeSatellites) {
            if (GUI.Button(new Rect(Screen.width - 20, height, 20, 20), new GUIContent("X"))) {
                DeleteSatellite(sat);
                return;
            }
            if (GUI.Button(new Rect(Screen.width - 120, height, 100, 20), new GUIContent(sat.Name)) && sat.IsActive) {
                _planetSelector.SelectObject(sat.Transform);
                return;
            }
            height += 30;
        }
    }
    private void DeleteSatellite(SatelliteInfo sat) {
        if (activeSatellites.Contains(sat)) {
            activeSatellites.Remove(sat);
            Destroy(sat.Transform.gameObject);
        }
    }
    private void AddSatellite(string name) {
        if (string.IsNullOrEmpty(name) || activeSatellites.Any(x=>x.Name == name) || !SatellitesMainController.DoesSatelliteExists(name)) return;
        GameObject prefab = Resources.Load("Prefabs/SatellitePrefab") as GameObject;
        GameObject satelliteObj = Instantiate(prefab, transform);
		Debug.Log(satelliteObj);
        SatelliteController satelliteController = satelliteObj.GetComponent<SatelliteController>();
		Debug.Log(satelliteController);
        satelliteController.OnEnabled = delegate () {
            if (_planetSelector != null)
                _planetSelector.SelectObject(satelliteController.transform);
        };
        satelliteController.RandomColor = _randomColor;
        satelliteController.SatelliteName = name;
        SatelliteInfo sat = SatellitesMainController.GetSatelliteInfo(name);
        satelliteController.SatPeriod = sat.Period;
        sat.Controller = satelliteController;
        activeSatellites.Add(sat);
    }
}