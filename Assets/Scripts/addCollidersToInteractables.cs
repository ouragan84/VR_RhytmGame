using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addCollidersToInteractables : MonoBehaviour
{
    public Vector3 center;
    void Start()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("UI Interactable")){
            BoxCollider c = obj.AddComponent<BoxCollider>();
            c.center = center;
            c.size = new Vector3( obj.GetComponent<RectTransform>().rect.width, obj.GetComponent<RectTransform>().rect.height, 1);
            c.isTrigger = true;
        }
    }
}
