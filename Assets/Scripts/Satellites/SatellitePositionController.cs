using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatellitePositionController {
    int _interpolationMult = 15;
    private SatellitePosition[] _positions;
    private SatellitePosition[] _interpolatedPositions;
    private double _period;
    private int _lastIndex;
    private bool _needUpdate = false;

    public SatellitePosition[] Positions
    {
        get
        {
            return _positions;
        }

        set
        {
            _positions = value;
            _needUpdate = false;
            Interpolate();
        }
    }

    private bool Interpolate()
    {
        //var s = _positions.SkipWhile(x => x.Time < startTime).ToArray();
        //var points = new List<SatellitePosition>();
        //foreach(var p in s)
        //{
        //    points.Add(p);
        //    if (p.Time > startTime + (decimal)_period) break;
        //}
        var points = _positions;
        if (points.Count() == 0)
            return false;
        float[][] inParams = new float[][]
        {
            points.Select(x=>(float)x.X).ToArray(),
            points.Select(x=>(float)x.Y).ToArray(),
            points.Select(x=>(float)x.Z).ToArray()
        };
        float[][] coordinates;
        TestMySpline.CubicSpline.FitParametric(inParams, _interpolationMult * points.Count(), out coordinates);
        _interpolatedPositions = new SatellitePosition[coordinates[0].Length];
        decimal step = (points.Last().Time - points.First().Time)/coordinates[0].Length;
        decimal sTime = points.First().Time;
        for (int i = 0; i < coordinates[0].Length; i++)
        {
            _interpolatedPositions[i] = new SatellitePosition()
            {
                Position = new Vector3(coordinates[0][i], coordinates[1][i], coordinates[2][i]),
                Time = sTime + step * i
            };
        }
        _lastIndex = 0;
        return true;
    }
    // Use this for initialization
    
    public bool NeedUpdate
    {
        get {
            return _positions == null ||
            _positions.Length == 0 ||
            _needUpdate;
        }
    }

    public double Period
    {
        get
        {
            return _period;
        }

        set
        {
            _period = value;
        }
    }

    public bool GetPosition(decimal time, out Vector3 pos)
    {
        pos = default(Vector3);
        //if (_interpolatedPositions.Length == 0 ||
        //    time < _interpolatedPositions[0].Time || 
        //    time > _interpolatedPositions[_interpolatedPositions.Length-1].Time)
        //{
        //    if (!Interpolate(time))
        //    {
        //        _needUpdate = true;
        //        return false;
        //    }
        //}
        //If index is last position
        while ((_lastIndex >= 0 && _lastIndex < _interpolatedPositions.Length-1)
            && !(_interpolatedPositions[_lastIndex].Time <= time && _interpolatedPositions[_lastIndex + 1].Time > time))
        {
            if (_interpolatedPositions[_lastIndex].Time > time) _lastIndex--;
            if (_interpolatedPositions[_lastIndex+1].Time < time) _lastIndex++;
        }
        if(_lastIndex <0 || _lastIndex >= _interpolatedPositions.Length - 1)
        {
            Interpolate();
            _needUpdate = true;
            return false;
        }
        float bias = (float)((time - _interpolatedPositions[_lastIndex].Time)/
            (_interpolatedPositions[_lastIndex+1].Time-_interpolatedPositions[_lastIndex].Time));
        pos = GetPointBetween(
            _interpolatedPositions[_lastIndex].Position, _interpolatedPositions[_lastIndex + 1].Position, bias
            );
        return true;
    }
    private Vector3 GetPointBetween(Vector3 f, Vector3 s, float p)
    {
        return f * (1-p) + s * p;
    }
    public SatellitePositionController(double period, int interMult = 15)
    {
        Period = period;
        _interpolationMult = interMult;
        _positions = new SatellitePosition[0];
        _interpolatedPositions = new SatellitePosition[0];
        _lastIndex = 0;
    }
    public Vector3[] GetPoints()
    {
        if (_interpolatedPositions.Length == 0 && _positions.Length != 0 && !_needUpdate) Interpolate();
        return _interpolatedPositions.Select(x => new Vector3((float)x.X, (float)x.Y, (float)x.Z)).ToArray();
    }
    public Vector3[] GetPoints(decimal start,decimal end)
    {
        return _interpolatedPositions.Where(x => x.Time >= start && x.Time <= end).Select(x=>x.Position).ToArray();
    }
}
