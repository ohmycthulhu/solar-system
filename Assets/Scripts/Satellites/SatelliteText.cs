using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SatelliteText : MonoBehaviour
{
    private void Start()
    {
        
    }
    private void Update()
    {
        var pos = transform.position - (Camera.allCameras.Where(x => x.enabled).First().transform.position - transform.position);
        transform.LookAt(pos);
    }
}
