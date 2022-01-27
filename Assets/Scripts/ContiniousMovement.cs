using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class ContiniousMovement : MonoBehaviour
{
    public XRNode inputSource;
    private Vector2 inputAxis;
    public XROrigin origin;
    //public XRRig rig;
    public float speed = 1f;
    public float gravity = -9.8f;
    public float additionalHeight = 0.2f;
    public LayerMask groundLayer;
    private float fallSpeed = 0f;
    private CharacterController character;
    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate(){
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, cam.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        character.Move(direction * Time.fixedDeltaTime * speed);

        //grav
        if(isGrounded())
            fallSpeed = 0;
        else{
            fallSpeed += gravity * Time.fixedDeltaTime;
            character.Move(Vector3.up*fallSpeed*Time.fixedDeltaTime);
        }
    }

    void CapsuleFollowHeadset(){
        character.height = origin.CameraInOriginSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(cam.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height/2 + character.skinWidth, capsuleCenter.z);
    }

    bool isGrounded(){
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
