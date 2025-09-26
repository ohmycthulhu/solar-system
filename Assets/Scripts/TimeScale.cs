using UnityEngine;
using System.Collections;

public class TimeScale : MonoBehaviour {
    private float maxTime, minTime, startTime;
	public GUIStyle styleHeader;
    public UnityEngine.UI.Button RealTimeButton;
    public UnityEngine.UI.Button NowButton;
    public UnityEngine.UI.Slider Slider;
    public UnityEngine.UI.InputField InputSpeed;
    private PlanetSelector _planetSelector;

    private IEnumerator Start() {
        maxTime = Mathf.Log(5) * 0.95f;
        minTime = -Mathf.Log(7200*24);
        startTime = -Mathf.Log(3600*24);
        _planetSelector = FindObjectOfType<PlanetSelector>();
        if (RealTimeButton != null)
            RealTimeButton.onClick.AddListener(SetRealTime);
        if(NowButton != null)
            NowButton.onClick.AddListener(delegate() {
                SetRealDate();
                StartCoroutine(_planetSelector.ClearTrails());
                });
        if (Slider != null) {
            Slider.onValueChanged.AddListener(SetTime);
            Slider.minValue = -(maxTime - minTime);
            Slider.maxValue = maxTime - minTime;
            yield return new WaitForEndOfFrame();
            SetRealTime();
        }
        else {
            TimeSystem.TimeScale = CalculateTimeScale(startTime - minTime);
        }
        if (InputSpeed != null) {
            InputSpeed.onEndEdit.AddListener(InsertSpeed);
        }
    }

    void SetRealTime() {
        Slider.value = startTime - minTime;
        TimeSystem.TimeScale = CalculateTimeScale(Slider.value);
    }

    void SetRealDate() {
        TimeSystem.Date = System.DateTime.UtcNow;
    }

    private void Update() {
        if (TimeSystem.NeedRestore) {
            Slider.value = startTime - minTime;
            TimeSystem.TimeScale = CalculateTimeScale(Slider.value);
        }
    }

    double CalculateTimeScale(float f) {
        float sign = Mathf.Sign(f);
        double v = System.Math.Exp(System.Math.Abs(f) + minTime);
        return sign * v;
    }

    void SetTime(float s) {
        TimeSystem.TimeScale = CalculateTimeScale(s);
    }

    void InsertSpeed(string t) {
        float speed;
        bool s = float.TryParse(t, out speed);
        if (!s) return;
        TimeSystem.TimeScale = speed/86400;
        Slider.value = (Mathf.Log(Mathf.Abs(speed/86400))-minTime) * Mathf.Sign(speed);
    }
}
