using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class Climber : MonoBehaviour
{
    private CharacterController character;
    public static XRController climbingHand;
    private ContiniousMovement continiousMovement;


    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        continiousMovement = GetComponent<ContiniousMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(climbingHand){
            continiousMovement.enabled = false;
            Climb();
        }
        else{
            continiousMovement.enabled = true;
        }
    }

    void Climb(){
        InputDevices.GetDeviceAtXRNode(climbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
        character.Move(transform.rotation * -velocity * Time.fixedDeltaTime);
    }
}
