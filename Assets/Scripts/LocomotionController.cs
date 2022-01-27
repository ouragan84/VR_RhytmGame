using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public bool leftTeleport {get; set;} = true;
    public bool rightTeleport {get; set;} = true;
    public XRController leftTeleportRay ;
    public XRController rightTeleportRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;
    public XRRayInteractor leftInteractorRay;
    public XRRayInteractor rightInteractorRay;


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3();
        Vector3 norm = new Vector3();
        int index = 0;
        bool validTarget = false;

        if(leftTeleportRay)
        {
            bool isLeftHover = leftInteractorRay.TryGetHitInfo(out pos, out norm, out index, out validTarget);
            leftTeleportRay.gameObject.SetActive(leftTeleport && CheckIfActivated(leftTeleportRay) && !isLeftHover);
        }

        if (rightTeleportRay)
        {
            bool isRightHover = rightInteractorRay.TryGetHitInfo(out pos, out norm, out index, out validTarget);
            rightTeleportRay.gameObject.SetActive(rightTeleport && CheckIfActivated(rightTeleportRay) && !isRightHover);
        }

    }


    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    }
}
