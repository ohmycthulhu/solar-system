using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCameraController : MonoBehaviour {
    int screenWidthSemiaxis, screenHeightSemiaxis;
    public float turnSpeed = Mathf.PI/4;
    public Transform focusObject = null;
    private Vector3 distance;
    public Transform CenterPoint {
        get {
            return focusObject;
        }
        set {
            focusObject = value;
            if (value == null) return;
            if (value.localPosition == Vector3.zero) {
                distance = (-value.position).normalized * CalculateDistance(value.lossyScale.magnitude);
            }
            else {
                distance = (-value.localPosition).normalized * CalculateDistance(value.lossyScale.magnitude);
            }
            if (distance == Vector3.zero) distance = Vector3.back * value.lossyScale.z * 1.5f;
            if (value.localPosition != Vector3.zero) distance *= -1;
            Vector3 position = focusObject.position + distance/*+ value.localPosition == Vector3.zero ? distance : -distance*/;
            GetComponent<Transform>().position = position;
            transform.LookAt(focusObject.position);
        }
    }
	
	void Start () {
        if (focusObject != null) CenterPoint = focusObject;
        screenWidthSemiaxis = Screen.width/2;
        screenHeightSemiaxis = Screen.height / 2;
	}
	
	
	void LateUpdate () {
        if (focusObject == null) return;
        Vector3 centerPos = focusObject.position;
        float x;
        if ((x = -Input.GetAxis("Mouse ScrollWheel")) != 0) {
            distance *= (1 + x);
        }
        if (Input.GetAxis("ResetCamera") != 0) {
            ResetCamera();
        }
        bool isButtonPressed = Input.GetAxis("Fire2") != 0;
        var turnAngle = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (isButtonPressed) {
            Vector3 mousePosition = Input.mousePosition;
            turnAngle += new Vector3(
                (mousePosition.x - screenWidthSemiaxis) / screenWidthSemiaxis,
                (mousePosition.y - screenHeightSemiaxis) / screenHeightSemiaxis);
        }
        transform.position = centerPos + distance;
        transform.LookAt(centerPos);
        if (Mathf.Abs(turnAngle.y) > 0.25)
            transform.RotateAround(focusObject.position, transform.right, turnAngle.y * turnSpeed);
        if (Mathf.Abs(turnAngle.x) > 0.25)
            transform.RotateAround(centerPos, transform.up, -turnAngle.x * turnSpeed);
        distance = transform.position - centerPos;
    }
    public void Reset() {
        
    }
    private float CalculateDistance(float size) {
        return 2.0f + 1.3f * size;
    }
    public void ResetCamera() {
        distance = (Vector3.zero - focusObject.position).normalized * CalculateDistance(focusObject.lossyScale.magnitude);
        if (distance == Vector3.zero) distance = Vector3.back * focusObject.lossyScale.z * 1.5f;
    }
}
