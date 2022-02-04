using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class getRayCastHitForVibration : MonoBehaviour
{
    XRController controller;
    bool vibratedLastFrame = false;
    public float intensity = .1f;
    public float duration = .05f;

    void Start(){
        controller = GetComponent<XRController>();
    }

    void Update(){
        RaycastHit hit;
        int layerMask = 1 << 5;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 30, layerMask)){
            if(hit.transform.gameObject.tag == "UI Interactable"){
                if(!vibratedLastFrame){
                    controller.SendHapticImpulse(intensity, duration);
                    vibratedLastFrame = true;
                }
            }else{
                vibratedLastFrame = false;
            }
        }else{
            vibratedLastFrame = false;
        }
    }
}
