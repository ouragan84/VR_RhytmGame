using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : ObstacleInterface
{
    private Collider collider;
    private bool hasHit = false;

    protected void Start(){
        base.Start();
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    new protected void FixedUpdate()
    {
        if(!isPaused){
            base.FixedUpdate();    

            if(!hasHit){

                bool HeadC = collider.bounds.Contains(VRHead.position);
                bool RightC = collider.bounds.Contains(VRRight.position);
                bool LeftC = collider.bounds.Contains(VRLeft.position);

                if(HeadC || RightC || LeftC)
                {
                    generator.TakeDamage(HeadC, RightC, LeftC); 

                    hasHit = true;
                    passedPlayer();
                }

                Destroy(gameObject, 8);
            }
        }
        
    }

    void Update()
    {
        if(!isPaused){
            if(!HasPassed && Vector3.Dot(speed, VRHead.position - transform.position) <= 0){
                //CalculateScore();
                passedPlayer();
                Destroy(gameObject, 8);
            }
        }
    }
}
