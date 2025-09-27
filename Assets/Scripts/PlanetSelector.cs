using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlanetSelector : MonoBehaviour {
    [SerializeField]
    public InputField dateInputField = null;
    public Transform[] planets;
    public Transform floatingCamera;
    public Transform mainCamera;
    public Transform freeCamera;
    public Transform[] DontScaleList;
    public ComboBox CameraSelector;
    private TextMesh[] texts;
    private IEnumerator clearTrails;
    int cameraChangeCooldown,planetChangeCooldown;
    string header = "Solar System";
    string text = "";
    public float planetScale = 8;
    private float nextScale;
    GUIStyle style;

    void Start () {
        if(planets.Count() == 0)
            planets = ((Transform[])FindObjectsOfType(typeof(Transform))).Where(x => (x.GetComponent<SphereCollider>() != null)).ToArray();
        Array.Sort(planets, (Transform x, Transform y) => { return x.position.magnitude.CompareTo(y.position.magnitude); });
        if (floatingCamera.GetComponent<FloatingCameraController>().CenterPoint == null) {
            floatingCamera.GetComponent<FloatingCameraController>().CenterPoint = planets[0];
        }
        cameraChangeCooldown = 0;
        planetChangeCooldown = 0;
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        text = String.Join("\n", planets.Select(x => x.name).ToArray());
        nextScale = planetScale;
        texts = FindObjectsOfType<TextMesh>().Where(x => x.GetComponent<SatelliteText>() == null).ToArray() ;
        clearTrails = ClearTrails();
        StartCoroutine(clearTrails);
        dateInputField.onEndEdit.AddListener(x=>UpdateDate(x));
        SelectFloatingCamera();
        if(CameraSelector != null) {
            CameraSelector.OnSelectionChanged = SelectCamera;
        }
    }
    private void UpdateDate(string date) {
        string[] formats = new string[] {
            "dd/MM/yyyy",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy H:mm:ss",
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy H:mm",
            "dd/MM/yyyy HH",
            "dd/MM/yyyy H" };
        DateTime dateToSet;
        Debug.Log(date);
        if ( DateTime.TryParseExact(date, formats,System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out dateToSet)) {
            Debug.Log(dateToSet);
            TimeSystem.Date = dateToSet;
        }
    }
    public IEnumerator ClearTrails() {
        yield return new WaitForEndOfFrame();
        var trails = FindObjectsOfType<TrailRenderer>();
        foreach (var trail in trails) {
            if (trail.enabled) {
                trail.Clear();
            }
        }
    }
    private void OnGUI() {
        if (!GUIController.Enable) return;
        string t = text;
        if (floatingCamera.GetComponent<FloatingCameraController>().CenterPoint != null) {
            string selectedPlanet = floatingCamera.GetComponent<FloatingCameraController>().CenterPoint.name;
            t = t.Replace(selectedPlanet, selectedPlanet + "\tX");
        }
        string date = "Date = " + TimeSystem.Date.ToString("dd/MM/yyyy HH:mm:ss");
        string speedInfo = "Speed = ";
        double speed = TimeSystem.TimeScale;
        if(System.Math.Abs(speed) < 1.0f) {
            speed *= 24;
            if(System.Math.Abs(speed) > 1.0f) {
                speedInfo = System.Math.Round(speed).ToString() + " h/s"; 
            } else {
                speed *= 60;
                if (System.Math.Abs(speed) > 1.0f) {
                    speedInfo = System.Math.Round(speed).ToString() + " m/s";
                } else {
                    speedInfo = System.Math.Round(speed * 60).ToString() + " s/s";
                }
            }
        }
        else {
            speedInfo += Math.Round(speed).ToString() + " day(s)/second";
        }
        GUI.Label(new Rect(10, 20, 200, 30), header);
        GUI.Label(new Rect(10, 40, 200, 20), date);
        GUI.Label(new Rect(10, 60, 200, 20), speedInfo);
        float height = 80, dh = 20;
        string[] buttons = t.Split('\n');
        for(int i = 0; i < buttons.Count();i++) {
            if(GUI.Button(new Rect(10, height, 100, dh), buttons[i])) {
                SelectPlanet(i);
            }
            height += dh;
        }
    }
    void SelectPlanet(int index) {
        if (index >= planets.Count()) return;
        floatingCamera.GetComponent<FloatingCameraController>().CenterPoint = planets[index];
        SelectFloatingCamera();
    }
    public void SelectObject(Transform p) {
        floatingCamera.GetComponent<FloatingCameraController>().CenterPoint = p;
        SelectFloatingCamera();
    }
    
    void Update () {

        cameraChangeCooldown -= cameraChangeCooldown != 0 ? 1 : 0;
        planetChangeCooldown -= planetChangeCooldown != 0 ? 1 : 0;
        if (Input.GetAxis("ChangePlanet") != 0 && planetChangeCooldown ==0) {
            SelectFloatingCamera();
            int indx = Array.LastIndexOf(planets, floatingCamera.GetComponent<FloatingCameraController>().CenterPoint);
            indx += Input.GetAxis("ChangePlanet") > 0 ? 1 : -1;
            indx = indx < 0 ? (planets.Length - Mathf.Abs(indx)) : (indx >= planets.Length ? (indx % planets.Length) : indx);
            floatingCamera.GetComponent<FloatingCameraController>().CenterPoint = planets[indx];
            planetChangeCooldown = 30;
        }
        if (Input.GetAxis("ChangeCamera") != 0 && cameraChangeCooldown == 0) {
            SelectCamera(Input.GetAxis("ChangeCamera") > 0);
            cameraChangeCooldown = 50;
        }
        if (Input.GetAxis("Fire1") != 0) {
            Camera camera = floatingCamera.GetComponent<Camera>().enabled ? floatingCamera.GetComponent<Camera>() : 
                (freeCamera.GetComponent<Camera>().enabled ? freeCamera.GetComponent<Camera>() : mainCamera.GetComponent<Camera>());
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, 1000000)) {
                floatingCamera.GetComponent<FloatingCameraController>().CenterPoint = hit.collider.transform;
                SelectFloatingCamera();
            }

        }
        
    }
    void SelectCamera(bool next) {
        // Floating => Free => Main
        if(floatingCamera.GetComponent<Camera>().enabled) {
            if (next) SelectFreeCamera();
            else SelectMainCamera();
        }
        else {
            if(mainCamera.GetComponent<Camera>().enabled) {
                if (next) SelectFloatingCamera();
                else SelectFreeCamera();
            }
            else {
                if (next) SelectMainCamera();
                else SelectFloatingCamera();
            }
        }
    }
    void SelectFloatingCamera() {
        Transform temp = floatingCamera.GetComponent<FloatingCameraController>().focusObject;
        floatingCamera.GetComponent<FloatingCameraController>().focusObject = null;
        foreach (var c in Camera.allCameras.Where(x => x.enabled)) {
            c.enabled = false;
        }
        floatingCamera.GetComponent<Camera>().enabled = true;
        if (planetScale < 1) ScalePlanets();
        ChangeTextVisibility(false);
        if (temp.GetComponent<TrailRenderer>() != null) {
            temp.GetComponent<TrailRenderer>().enabled = false;
        }
        if(temp.transform.parent.GetComponent<TrailRenderer>() != null) {
            temp.transform.parent.GetComponent<TrailRenderer>().enabled = false;
        }

        foreach(var tr in FindObjectsOfType<TrailRenderer>()) {
            if(!tr.enabled && tr.transform != temp)
                tr.enabled = true;
        }
        /*if(floatingCamera.GetComponent<FloatingCameraController>().GetComponent<TrailRenderer>() != null) {
            floatingCamera.GetComponent<FloatingCameraController>().GetComponent<TrailRenderer>().enabled = false;
        }*/
        floatingCamera.GetComponent<FloatingCameraController>().focusObject = temp;
        CameraSelector.SelectedIndex = 1;
    }
    void SelectMainCamera() {
        foreach (var c in Camera.allCameras.Where(x => x.enabled)) {
            c.enabled = false;
        }
        //floatingCamera.GetComponent<Camera>().enabled = false;
        //freeCamera.GetComponent<Camera>().enabled = false;
        mainCamera.GetComponent<Camera>().enabled = true;
        if (planetScale > 1) ScalePlanets();
        ChangeTextVisibility(false);
        foreach (var tr in FindObjectsOfType<TrailRenderer>()) {
            tr.enabled = true;
        }
        CameraSelector.SelectedIndex = 0;
    }
    void SelectFreeCamera() {
        var cameras = Camera.allCameras.Where(x=>x.enabled);
        if (cameras.Count() != 0)  {
            Camera active = cameras.First();
            freeCamera.position = active.transform.position;
            freeCamera.rotation = active.transform.rotation;
            active.enabled = false;
        }
        foreach(var c in Camera.allCameras.Where(x => x.enabled)) {
            c.enabled = false;
        }
        freeCamera.GetComponent<Camera>().enabled = true;
        if (planetScale < 1) ScalePlanets();
        ChangeTextVisibility(false);
        foreach (var tr in FindObjectsOfType<TrailRenderer>()) {
            tr.enabled = true;
        }
        CameraSelector.SelectedIndex = 2;
    }
    void ChangeTextVisibility(bool visible) {
        foreach(var t in texts) {
            t.gameObject.SetActive(visible);
        }
    }
    void ScalePlanets() {
       foreach(Transform t in planets) {
            if (t == null) continue;
           if (!DontScaleList.Contains(t)) t.localScale *= planetScale;
        }
       planetScale = 1 / planetScale;
    }
    void SelectCamera(int index) {
        switch (index) {
            case 0:
                SelectMainCamera();
                return;
            case 1:
                SelectFloatingCamera();
                return;
            case 2:
                SelectFreeCamera();
                return;
            default:
                return;
        }
    }
}
