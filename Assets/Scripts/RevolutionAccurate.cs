using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RevolutionAccurate : MonoBehaviour {
    public string identifier;
    public float timeScale = 1.0f;
    public float scale = 1.0f;
    Orbit orbit;
	// Use this for initialization
	void Start () {
        orbit = PositionsStorage.GetOrbit(identifier);
        transform.localPosition = GetPosition(0)/scale;
	}
	
	// Update is called once per frame
	void Update () {    
        if (orbit == null) return;
        transform.localPosition = GetPosition((float)TimeSystem.Time/timeScale) / scale;
	}
    private Vector3 GetPosition(float time)
    {
        Vector3 position = Vector3.zero;
        float angleBase = (time * 2 * Mathf.PI / orbit.Period);
        position.x = orbit.SemiAxes.x * Mathf.Cos(angleBase + orbit.PhaseOffset.x);
        position.y = orbit.SemiAxes.y * Mathf.Cos(angleBase + orbit.PhaseOffset.y);
        position.z = orbit.SemiAxes.z * Mathf.Cos(angleBase + orbit.PhaseOffset.z);
        position += orbit.Center;
        return position;
    }
}
