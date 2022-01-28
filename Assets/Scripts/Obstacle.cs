using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static Vector3 speed;
    public static Transform VRHead;
    public static Transform VRLeft;
    public static Transform VRRight;
    private bool HasPassed = false;
    public static WallGenerator generator;
    private Collider collider;
    private bool hasHit = false;
    public static bool followCurve;
    public static AnimationCurve speedMultiplier;
    private float timeCreated;

    void Start(){
        collider = GetComponent<Collider>();
        timeCreated = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float mult = followCurve?speedMultiplier.Evaluate(Time.time-timeCreated):1;
        transform.Translate(speed*mult*Time.fixedDeltaTime);

        if(!hasHit){
            
            bool HeadC = collider.bounds.Contains(VRHead.position);
            bool RightC = collider.bounds.Contains(VRRight.position);
            bool LeftC = collider.bounds.Contains(VRLeft.position);

            if(HeadC || RightC || LeftC)
            {
                generator.TakeDamage(HeadC, RightC, LeftC);

                hasHit = true;
                HasPassed = true;
            }
            
            Destroy(gameObject, 8);
        }
    }

    void Update()
    {
        if(!HasPassed && Vector3.Dot(speed, VRHead.position - transform.position) <= 0){
            //CalculateScore();
            HasPassed = true;
            Destroy(gameObject, 8);
        }
    }

    int DistanceToScore(float dis){
        float mult = (250-50)/Mathf.Pow(5-30, 2);
        return (dis <= 5.0? 500 : (dis > 30.0? 0 : Mathf.FloorToInt(mult*Mathf.Pow(dis-30,2)+50)));  //floor(500/(x^2+1))
    }
}
