using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleInterface : MonoBehaviour
{
    public static Vector3 speed;
    public static Transform VRHead;
    public static Transform VRLeft;
    public static Transform VRRight;
    protected bool HasPassed = false;
    public static WallGenerator generator;
    public static bool followCurve;
    public static AnimationCurve speedMultiplier;
    protected float timeCreated;
    public static bool isPaused = false;
    public static float totalTimePaused = 0.0f;

    protected void Start(){
        timeCreated = Time.time - totalTimePaused;
    }

    protected void FixedUpdate(){
        float mult = followCurve?speedMultiplier.Evaluate(Time.time-timeCreated-totalTimePaused):1;
        transform.Translate(speed*mult*Time.fixedDeltaTime);
    }
}
