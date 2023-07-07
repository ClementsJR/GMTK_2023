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
    SoundEffector gunshotEffect;

    void Update() {
        if(Input.GetButtonDown("Jump")) {
            body.AddForceAtPosition(transform.forward * fireForce, muzzle.position);
            gunshotEffect.PlaySoundEffect();
		}
    }
}
