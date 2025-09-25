using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimeControl : MonoBehaviour {
    // Use this for initialization
    public float timeScale = 1.0f;
    public float timeInterval = 0.05f;
    private Thread timeUpdater;
    void UpdateTime()
    {
        while (true)
        {
            Thread.Sleep((int)(timeInterval * 1000));
            TimeSystem.Update(timeInterval);
        }
    }
	void Start () {
        TimeSystem.Initialize(0,timeScale);
        timeUpdater = new Thread(new ThreadStart(UpdateTime));
        timeUpdater.Start();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //TimeSystem.Update(Time.fixedUnscaledTime);
        //Debug.Log(Time.unscaledDeltaTime);
	}
}
