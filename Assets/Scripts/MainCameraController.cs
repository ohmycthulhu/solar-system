using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour {
    Vector3 startPosition;
    Vector3 startRotation;
    public float rotationSpeed = 3.0f;
	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetAxis("ResetCamera") != 0)
        {
            ResetCamera();
        }
        Vector3 rotation = new Vector3();
        rotation.y = Mathf.Abs(Input.GetAxis("Vertical")) > 0.2 ? Input.GetAxis("Vertical") * rotationSpeed : 0.0f;
        rotation.x = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2 ? Input.GetAxis("Horizontal") * rotationSpeed : 0.0f;
        Vector3 position = transform.position * (1-Input.GetAxis("Mouse ScrollWheel"));
        transform.position = position;
        transform.RotateAround(Vector3.zero,transform.up, -rotation.x);
        transform.RotateAround(Vector3.zero,transform.right, rotation.y);
	}
    public void ResetCamera()
    {
        transform.position = startPosition;
        transform.LookAt(Vector3.zero);
    }
}
