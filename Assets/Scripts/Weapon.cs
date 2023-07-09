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
    [SerializeField]
    Vector3 bulletSpread = new Vector3(0.1f, 0.1f, 0.1f);

    [Header("Audio")]
    [SerializeField]
    SoundEffector firingSound;

    [Header("Visual")]
    [SerializeField]
    TrailRenderer bulletTrail;
    [SerializeField]
    float bulletSpeed = 200f;

    private float timeToFireAgain;
    private GenericController controller;

	private void Start() {
        timeToFireAgain = 0f;
        controller = gameObject.GetComponentInParent<GenericController>();
	}

	private void Update() {
		if (timeToFireAgain > 0f) {
            timeToFireAgain -= Time.deltaTime;
		}
    }

    public bool CanFire() {
        return timeToFireAgain <= 0f;
    }

    public void Fire(Vector3 direction) {
        if (timeToFireAgain > 0f) return;

        Vector3 spreadDirection = direction + GetSpread();
        spreadDirection.Normalize();

        timeToFireAgain = fireRate;
        firingSound.Play();
        TrailRenderer trail = Instantiate(bulletTrail, muzzle.position, Quaternion.identity);
        Vector3 trailEndpoint = direction * 500f;

        if (Physics.Raycast(muzzle.position, spreadDirection, out RaycastHit hit)) {
            if (hit.rigidbody != null) {
                hit.rigidbody.AddForceAtPosition(spreadDirection * bulletForce, hit.point);

                hit.rigidbody.BroadcastMessage("DealDamage", this);
            }
            trailEndpoint = hit.point;
        }

        StartCoroutine(SpawnTrail(trail, trailEndpoint));
    }

    public float Damage() {
        return damage;
	}

    public GenericController Attacker() {
        return controller;
	}

    public Transform Muzzle() {
        return muzzle;
	}

    private Vector3 GetSpread() {
        return new Vector3(
            Random.Range(-bulletSpread.x, bulletSpread.x),
            Random.Range(-bulletSpread.y, bulletSpread.y),
            Random.Range(-bulletSpread.z, bulletSpread.z)
        );
	}

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endPoint) {
        Vector3 startPoint = muzzle.position;
        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0) {
            trail.transform.position = Vector3.Lerp(startPoint, endPoint, 1 - (remainingDistance / distance));
            remainingDistance -= bulletSpeed * Time.deltaTime;
            yield return null;
		}

        Destroy(trail.gameObject);
    }
}
