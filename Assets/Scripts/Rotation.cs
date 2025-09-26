using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
    public enum Direction {
        PITCH,
        ROLL,
        YAW
    }
    public Direction direction = Direction.YAW;
    [SerializeField]
    private double _period = 1;
    private Vector3 _startAngles;
    private void Start() {
        _startAngles = transform.eulerAngles;
    }
    System.DateTime _lastDate;

    public double Period {
        get {
            return _period;
        }

        set {
            _period = value;
        }
    }

    void FixedUpdate () {
        Vector3 dir = new Vector3();
        switch (direction) {
            case Direction.PITCH:
                dir = Vector3.right;
                break;
            case Direction.ROLL:
                dir = Vector3.forward;
                break;
            case Direction.YAW:
                dir = -Vector3.up;
                break;
        }
        Vector3 angles = dir * 360 * 
            (float)(
            ((TimeSystem.Time - System.Math.Floor(TimeSystem.Time / (decimal)_period) * (decimal)_period))/(decimal)_period);
        if (_lastDate != null) {
            if(_lastDate.ToShortDateString() != TimeSystem.Date.ToShortDateString()) {
            }
        }
        _lastDate = TimeSystem.Date;
        transform.eulerAngles = _startAngles + angles;
    }
}
