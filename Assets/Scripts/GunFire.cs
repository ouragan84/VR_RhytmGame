using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    public float shootSpeed = 40;
    public GameObject bullet;
    public Transform barrel;
    public AudioSource audioSource;
    public AudioClip fireAudio;

    public void Fire(){
        GameObject bul = Instantiate(bullet, barrel.position, barrel.rotation);
        bul.GetComponent<Rigidbody>().velocity = shootSpeed * barrel.forward;
        audioSource.PlayOneShot(fireAudio);
        Destroy(bul, 3);
    }
}
