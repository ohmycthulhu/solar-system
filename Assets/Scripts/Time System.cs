using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeSystem{
    static decimal startTimePoint = 2451544.5m;
    static DateTime startDate = new DateTime(2000, 1, 1, 12, 0, 0,DateTimeKind.Utc);
    static DateTime currentDate = startDate;
    static decimal timeCurrent = startTimePoint;
    static double deltaTime = 0.0;
    static bool needToRestore = false;
    static double tScale = 1.0;
    static public double TimeScale {
        get { return tScale; }
        set { tScale = value; }
    }

    static public bool NeedRestore {
        get {
            bool result = needToRestore;
            if (needToRestore) needToRestore = false;
            return result;
        }
    }

    static public DateTime Date {
        get { return currentDate;  }
        set {
            if (value < startDate) return;
            var date = (value - startDate);
            var seconds = (value - startDate).TotalSeconds;
            timeCurrent = (startTimePoint + (decimal)seconds/86400); // 86400 = 3600 * 24 
            currentDate = value;
            Debug.Log(currentDate);
            Debug.Log(timeCurrent);
        }
    }

    static public decimal Time {
        get { return timeCurrent; }
        set { timeCurrent = value; CalculateCurrentDate(); }
    }

    static public double DeltaTime {
        get { return deltaTime; }
    }

    static public void Initialize(float startTime, float scale = 1.0f) {
        timeCurrent = startTimePoint + (decimal)startTime;
        TimeScale = scale;
        currentDate.AddDays((double)(timeCurrent - startTimePoint));
    }

    static public void AddTime(float t) {
        timeCurrent += (decimal)t;
        currentDate.AddDays(t);
    }

    static public void Update(float unscaledDelta) {
        if (timeCurrent <= 0) {
            TimeScale = 1;
            needToRestore = true;
            return;
        }
        decimal dT = ((decimal)unscaledDelta * (decimal)TimeScale);
        deltaTime = (double)dT;
        timeCurrent += dT;
        currentDate = currentDate.AddDays((double)dT);
    }


    public static decimal ToUniversalUnit(DateTime date) {
        return (decimal)(date - startDate).TotalDays+ startTimePoint;
    }

    static void CalculateCurrentDate() {
        currentDate = startDate.AddDays((double)(timeCurrent - startTimePoint));
    }
    public static void SetRealTime() {
        TimeScale = 1 / 86400.0;
    }
}
