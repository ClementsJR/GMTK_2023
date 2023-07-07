using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    [SerializeField]
    private Rigidbody body;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private float fireForce = 1000f;

    [SerializeField]
    SoundEffector gunshot;

    void Update() {
        if(Input.GetButtonDown("Jump")) {
            //body.AddForce(transform.forward * fireForce);
            body.AddForceAtPosition(transform.forward * fireForce, muzzle.position);
            gunshot.PlaySoundEffect();
		}
    }
}
