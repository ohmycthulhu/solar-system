using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour {
    //float currentSpeed;
    //Vector2 rotationSpeed;
    public float rotationSpeed = 0.5f;
    public float movementSpeed = 5.0f;
    
	void Start () {
        //currentSpeed = 0;
        //rotationSpeed = Vector2.zero;
	}
	
	
	void Update () {
        Vector2 rotation = Vector2.zero;
        float movement = 0.0f;
        if (Input.GetAxis("Vertical") != 0) {
            movement += movementSpeed * Mathf.Sign(Input.GetAxis("Vertical")) * Time.fixedDeltaTime;
        }
        if(Input.GetAxis("Horizontal") != 0) {
            rotation.y += Mathf.Sign(Input.GetAxis("Horizontal")) * rotationSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetAxis("Pitch") != 0) {
            rotation.x += Mathf.Sign(Input.GetAxis("Pitch")) * rotationSpeed * Time.fixedDeltaTime;
        }
        transform.position += transform.forward * movement;
        transform.Rotate(new Vector3(-rotation.x, rotation.y));
	}
    public void ResetCamera() {

    }
}
