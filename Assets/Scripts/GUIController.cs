using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour {
    static private bool enable = true;
    int guiChangeCooldown;
    static public bool Enable {
        get { return enable; }
        set { enable = value;ChangeGUIState(value); }
    }
	
	void Start () {
        guiChangeCooldown = 0;
    }
	
	
	void Update () {
        if (guiChangeCooldown != 0) guiChangeCooldown -= 1;
        if (Input.GetAxis("ChangeGUIState") != 0 && guiChangeCooldown <= 0) {
            Enable ^= true;
            guiChangeCooldown = 16;
        }
	}
    static void ChangeGUIState(bool state) {
        var canvas = FindObjectsOfType<Canvas>();
        foreach(var c in canvas) {
            c.enabled = state;
        }
    }
}
