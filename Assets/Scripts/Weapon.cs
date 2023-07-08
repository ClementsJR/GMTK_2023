using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    [Header("Weapon Stats")]
    [SerializeField]
    Transform muzzle;
    [SerializeField][Range(0f, 500f)]
    float bulletForce = 250f;
    [SerializeField][Range(0f, 1f)]
    float fireRate = 0.2f;
    [SerializeField][Range(0f, 5f)]
    float damage = 1f;

    [Header("Audio")]
    [SerializeField]
    SoundEffector firingSound;

    private float timeToFireAgain;

	private void Start() {
        timeToFireAgain = 0f;
	}

	private void Update() {
		if (timeToFireAgain > 0f) {
            timeToFireAgain -= Time.deltaTime;
		}
	}

	public void Fire(Vector3 direction) {
        if (timeToFireAgain > 0f) return;

        firingSound.Play();
        timeToFireAgain = fireRate;

        Color lineColor = Color.red;
        if (Physics.Raycast(muzzle.position, direction, out RaycastHit hit)) {
            if (hit.rigidbody != null) {
                hit.rigidbody.AddForceAtPosition(direction * bulletForce, hit.point);
                lineColor = Color.green;

                hit.rigidbody.BroadcastMessage("DealDamage", damage);
            }
            Debug.DrawLine(muzzle.position, hit.point, lineColor, 2f);
        } else {
            Debug.DrawRay(muzzle.position, direction * 100f, Color.white, 2f);
        }
    }

    public bool CanFire() {
        return timeToFireAgain <= 0f;
	}
}
