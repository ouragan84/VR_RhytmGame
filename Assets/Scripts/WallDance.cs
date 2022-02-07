using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDance : ObstacleInterface
{
    public Transform HeadTarget;
    public Transform LeftTarget;
    public Transform RightTarget;
    new protected void Start(){
        base.Start();
        transform.localScale = generator.transform.localScale;
        transform.position = generator.transform.position;

        HeadTarget = transform.Find("Head");
        LeftTarget = transform.Find("Left");
        RightTarget = transform.Find("Right");
    }

    void Update()
    {
        if(!isPaused){
            if(!HasPassed && Vector3.Dot(speed, VRHead.position - transform.position) <= 0){
                CalculateScore();
                passedPlayer();
                Destroy(gameObject, 8);
            }
        }
    }

    new protected void FixedUpdate()
    {
        if(!isPaused){
            base.FixedUpdate();
        }
    }

    void CalculateScore(){
        float headDis =  Vector3.Magnitude(Vector3.ProjectOnPlane(VRHead.position-HeadTarget.position,speed))*100;//in cm
        float rightDis = Vector3.Magnitude(Vector3.ProjectOnPlane(VRRight.position-RightTarget.position,speed))*100;
        float leftDis = Vector3.Magnitude(Vector3.ProjectOnPlane(VRLeft.position-LeftTarget.position,speed))*100;

        generator.addScore(headDis, rightDis, leftDis);
    }
}
