using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolutionSimple : MonoBehaviour {
    public float period = 24;
    float distance;
	// Use this for initialization
	void Start () {
        distance = Mathf.Sqrt(transform.localPosition.sqrMagnitude);
    }
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = GetPosition((float)TimeSystem.Time * 2 * Mathf.PI / period);
	}
    Vector3 GetPosition(float angle)
    {
        return new Vector3(distance * Mathf.Cos(angle), transform.localPosition.y, distance * Mathf.Sin(angle));
    }
}
