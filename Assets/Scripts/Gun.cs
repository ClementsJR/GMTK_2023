using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour {

    [SerializeField]
    private Rigidbody body;
    [SerializeField]
    private Transform muzzle;

    [SerializeField]
    private float fireForce = 1000f;
    [SerializeField]
    private int ammunition = 20;

    [SerializeField] SoundEffector gunshotEffect;
    [SerializeField] SoundEffector noAmmoEffect;
    [SerializeField] TextMeshProUGUI ammoDisplay;

	private void Start() {
        UpdateAmmoDisplay();
	}

	void Update() {
        if(Input.GetButtonDown("Jump")) {
            if(ammunition > 0) {
                body.AddForceAtPosition(transform.forward * fireForce, muzzle.position);
                gunshotEffect.Play();

                ammunition--;
                UpdateAmmoDisplay();
            } else {
                Debug.Log("No Ammo");
                //noAmmoEffect.Play();
			}
		}
    }

    void UpdateAmmoDisplay() {
        ammoDisplay.text = "Ammo: " + ammunition;
	}
}
