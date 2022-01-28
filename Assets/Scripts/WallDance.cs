using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDance : MonoBehaviour
{
    //800 x 600
    public static Vector3 speed;
    public Transform HeadTarget;
    public Transform LeftTarget;
    public Transform RightTarget;
    public static Transform VRHead;
    public static Transform VRLeft;
    public static Transform VRRight;
    private bool HasPassed = false;
    public static WallGenerator generator;
    public static bool followCurve;
    public static AnimationCurve speedMultiplier;
    private float timeCreated;

    void Start(){
        transform.localScale = generator.transform.localScale;
        timeCreated = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float mult = followCurve?speedMultiplier.Evaluate(Time.time-timeCreated):1;
        transform.Translate(speed*mult*Time.fixedDeltaTime);
    }

    void Update()
    {
        if(!HasPassed && Vector3.Dot(speed, VRHead.position - transform.position) <= 0){
            CalculateScore();
            HasPassed = true;
            Destroy(gameObject, 8);
        }
    }

    void CalculateScore(){
        float headDis =  Vector3.Magnitude(Vector3.ProjectOnPlane(VRHead.position-HeadTarget.position,speed))*100;//in cm
        float rightDis = Vector3.Magnitude(Vector3.ProjectOnPlane(VRRight.position-RightTarget.position,speed))*100;
        float leftDis = Vector3.Magnitude(Vector3.ProjectOnPlane(VRLeft.position-LeftTarget.position,speed))*100;

        generator.addScore(headDis, rightDis, leftDis);
    }
}
