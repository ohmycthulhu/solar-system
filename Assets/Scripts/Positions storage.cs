using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Orbit
{
    private Vector3 semiAxes;
    private Vector3 center;
    private float period;
    private Vector3 phaseOffset;
    public Vector3 SemiAxes
    {
        get
        {
            return semiAxes;
        }
        set
        {
            semiAxes = value;
        }
    }
    public Vector3 Center
    {
        get
        {
            return center;
        }
        set
        {
            center = value;
        }
    }
    public Vector3 PhaseOffset
    {
        get
        {
            return phaseOffset;
        }
        set
        {
            phaseOffset= value;
        }
    }
    public float Period
    {
        get
        {
            return period;
        }
        set
        {
            period= value;
        }
    }
    public Orbit() { }
    public Orbit(Vector3 sA, Vector3 c, float p, Vector3 pO) { semiAxes = sA;center = c; period = p; phaseOffset = pO; }
}
public static class PositionsStorage {
    static Dictionary<string,List<Vector3>> points = new Dictionary<string, List<Vector3>>(){
            {
                "mercury", new List<Vector3>(){
                    new Vector3(0.111f,-0.43f),
                    new Vector3(0.3205f,-0.23f)
                }
            },
            {
                "venus", new List<Vector3>(){
                    new Vector3(-0.384019f,-0.6144f),
                    new Vector3(-0.1441f,0.70426f)
                }
            },
            {
                "earth",new List<Vector3>()
                {
                    new Vector3(0.6050702901916951f,-0.8085113449604454f),
                    new Vector3(0.8109517301254524f,0.5806949600758906f),
                }
            },            
            {
                "mars", new List<Vector3>(){
                    new Vector3(0.996880f,1.074675f),
                    new Vector3(-1.539363f,-0.512287f)
                }
            },
            {
                "jupiter", new List<Vector3>(){
                    new Vector3(-3.13617f,-4.3769f),
                    new Vector3(1.6045f,-4.9108f),
                }
            },
            {
                "saturn", new List<Vector3>(){
                    new Vector3(14.293f,-9.953f),
                    new Vector3(6.566999f,-7.44987f)
                }
            },
            {
                "uranus", new List<Vector3>(){
                    new Vector3(17.22f,0.923f),
                    new Vector3(6.304708f,18.24f)
                }
            },
           {
                "neptun", new List<Vector3>(){
                    new Vector3(12.773113350f,14.91f),
                    new Vector3(6.535f,18.180f)
                }
            }
        };
    static Dictionary<string, Orbit> orbits = new Dictionary<string, Orbit>()
    {
        {
                "mercury", new Orbit(
                        new Vector3(0.3761988f,0.0461603f,0.3761988f),
                        new Vector3(-0.0267928f,-0.0070126f,-0.1159040f),
                        87.6f,
                        new Vector3(1.6454246f,-0.7581156f,0.0862135f))
            },
            {
                "venus", new Orbit(
                        new Vector3(0.72182443f,0.04276757f,0.72207454f),
                        new Vector3(0.00559851f,-0.00040993f,-0.00646989f),
                        266.6f,
                        new Vector3(0.90435844f,-2.00347260f,-0.66124951f))
            },
            {
                "earth",new Orbit(
                        new Vector3(0.9993368137f,0.0000449854f,0.9998402437f),
                        new Vector3(0.0048362675f,0.0000014918f,-0.0239340297f),
                        365.24f,
                        new Vector3(-1.8625491413f,-0.0051213957f,2.8508022986f))
            },
            {
                "mars", new Orbit(
                        new Vector3(1.5037177f,0.0484218f,1.5010513f),
                        new Vector3(-0.1967800f,0.0066098f,0.0858616f),
                        686.67f,
                        new Vector3(1.5997704f,-0.8336816f,0.0276816f))
            },
            {
                "jupiter", new Orbit(
                        new Vector3(5.1941449f,0.1180539f,5.1954525f),
                        new Vector3(-0.3669198f,0.0085941f,-0.0941475f),
                        4331.7464f,
                        new Vector3(4.0745180f,0.7468166f,2.5034373f))
            },
            {
                "saturn", new Orbit(
                        new Vector3(9.516770f,0.413690f,9.529418f),
                        new Vector3(0.030958f,0.012221f,-0.777211f),
                        10759.9704f,
                        new Vector3(0.815335f,-2.735110f,-0.755892f))
            },
            {
                "uranus", new Orbit(
                        new Vector3(19.261513f,0.259337f,19.204992f),
                        new Vector3(1.385118f,-0.01877f,-0.23088f),
                        30683.8124f,
                        new Vector3(-0.513435f,2.905788f,-2.086460f))
            },
           {
                "neptune", new Orbit(
                        new Vector3(30.059859f,0.929673f,30.069789f),
                        new Vector3(-0.310390f,0.012581f,-0.263146f),
                        60191.552f,
                        new Vector3(0.564545f,2.976996f,-1.005883f))
            }
    };
    static public float scale = 2250.0f;

    public  static List<Vector3> GetPoints(string name){
        return points[name.ToLower()];
    }
    public static Vector3 GetSemiAxis(string name,Vector3 center){
        return scale * CalculateAxis(new Vector3[] { points[name.ToLower()][0], points[name.ToLower()][1] },center);
    }
    public static Orbit GetOrbit(string name)
    {
        
        if (!orbits.ContainsKey(name.ToLower())) return null;
        Orbit o = orbits[name.ToLower()];
        o.Center *= scale;
        o.SemiAxes *= scale;
        return o;
    }
    private static Vector3 CalculateAxis(Vector3[] points, Vector3 center){
        Vector3 solutions = new Vector3();
        double x1 = points[0].x, x2 = points[1].x;
        double y1 = points[0].y, y2 = points[1].y;
        double lambda = Math.Pow(x1 * y2,2) - Math.Pow(x2 * y1,2);
        solutions.x = (float)(1 / Math.Sqrt(Math.Abs(((Math.Pow(y2, 2) - Math.Pow(x2, 2)) / lambda))));
        solutions.z = (float)(1 / Math.Sqrt(Math.Abs(((Math.Pow(x1, 2) - Math.Pow(y1, 2)) / lambda))));
        return solutions;
    }
}
