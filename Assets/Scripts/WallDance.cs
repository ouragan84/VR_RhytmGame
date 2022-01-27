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

    void Start(){
        transform.localScale = generator.transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(speed*Time.fixedDeltaTime);
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
