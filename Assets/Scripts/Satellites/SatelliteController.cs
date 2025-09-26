using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SatelliteController : MonoBehaviour {
    SatellitePositionController _positionController;
    private string _satName = "";
    private bool _active;
    [SerializeField]
    private double _satPeriod = 1.0;
    [SerializeField]
    private int _informationInterval = 10;
    [SerializeField]
    private Color _color;
    [SerializeField]
    private bool _randomColor;
    private const float _distanceCoef = 960000;

    // TODO: Rename to _onEnabled
    private UnityAction onEnabled = null;
    private decimal _lastOrbitUpdate = 0;
    public string SatelliteName {
        get { return _satName; }
        set { _satName = value; name = value; }
    }

    public int InformationInterval {
        get {
            return _informationInterval;
        }

        set {
            _informationInterval = value > 0 ? value : 14;
        }
    }

    public Color Color {
        get {
            return _color;
        }

        set {
            _color = value;
            _line.startColor = value;
            _line.endColor = value;
            _text.color = value;
        }
    }

    // TODO: Remove accessor
    public bool RandomColor {
        get {
            return _randomColor;
        }

        set {
            _randomColor = value;
        }
    }

    public double SatPeriod {
        get {
            return _satPeriod;
        }

        set {
            _satPeriod = value;
            if(_positionController != null)
                _positionController.Period = value;
        }
    }
    public UnityAction OnEnabled {
        get {
            return onEnabled;
        }

        set {
            onEnabled = value;
        }
    }
    public bool Active {
        get {
            return _active;
        }
    }
    TextMesh _text;
    LineRenderer _line;
    int _lastUsedEquation;
    bool _isUpdating = false;

    void Start () {
        _lastUsedEquation = 0;
        SatellitesMainController.Initialize();
        _positionController = new SatellitePositionController(_satPeriod);
        _line = GetComponentInChildren<LineRenderer>();
        _line.name = name + " orbit";
        _line.transform.SetParent(transform.parent,false);
        _text = GetComponentInChildren<TextMesh>();
        _text.text = _satName;
        _text.color = _color;
        if(_randomColor) {
            Color = new Color(Random.value, Random.value, Random.value);
        }
        transform.localPosition = Vector3.zero;
        _text.gameObject.SetActive(_active);
	}
	void UpdateSatelliteData() {
        if (_isUpdating) return;
        StartCoroutine(_UpdateInfo(TimeSystem.Time-InformationInterval*(decimal)_satPeriod,TimeSystem.Time+InformationInterval*(decimal)_satPeriod));
    }

    IEnumerator _UpdateInfo(decimal start_time, decimal end_time) {
        _isUpdating = true;
        yield return SatellitesMainController.PrepareSatelliteInfo(this.SatelliteName, start_time, end_time, 1000);
        _positionController.Positions = SatellitesMainController.GetDownloadsResult(SatelliteName);
        _lastUsedEquation = 0;
        _isUpdating = false;
    }

    void Update () {
        Vector3 pos;
        if(_positionController.GetPosition(TimeSystem.Time,out pos)) {
            transform.localPosition = pos / _distanceCoef;
            if(_lastOrbitUpdate <= TimeSystem.Time-(decimal)_satPeriod / 4) {
                UpdateOrbit();
            }
            EnableSatellite();
        }
        else {
            DisableSatellite();
        }
        if(_positionController.NeedUpdate) {
            UpdateSatelliteData();
        }
	}

    void DisableSatellite() {
        if (!_active) return;
        _text.gameObject.SetActive(false);
        _line.enabled = false;
        _active = false;
        transform.localPosition = Vector3.zero;
    }

    void EnableSatellite() {
        if(onEnabled != null) {
            onEnabled();
            onEnabled = null;
        }
        if (_active) return;
        _text.gameObject.SetActive(true);
        _line.enabled = true;
        _active = true;
        UpdateOrbit();
    }

    void UpdateOrbit() {
        _lastOrbitUpdate = TimeSystem.Time;
        Vector3[] points = _positionController.GetPoints(TimeSystem.Time - (decimal)_satPeriod*0.75m,TimeSystem.Time + (decimal)_satPeriod*0.75m);
        if(points != null && points.Length != 0) {
            _line.positionCount = points.Length;
            for (int i = 0; i < points.Length; i++) _line.SetPosition(i, points[i]/_distanceCoef);
        }
    }

    private void OnDestroy() {
        Destroy(_line.gameObject);
    }
}
