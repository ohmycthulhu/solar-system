using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailController : MonoBehaviour {
    TrailRenderer tr;
    bool restored;
    float time;
    IEnumerator ie;
	
	void Start () {
        tr = GetComponent<TrailRenderer>();
        time = tr.time;
        tr.time = 0.0001f;
        ie = Restore();
        StartCoroutine(ie);
    }
    IEnumerator Restore() {
        yield return new WaitForSeconds(1);
        tr.time = time;
    }
}
