using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour {

    [Header("Phyics Bodies")]
    [SerializeField]
    Rigidbody gun;
    [SerializeField]
    Rigidbody man;

    [Header("Explosion Options")]
    [SerializeField]
    float explosionForce = 1000f;
    [SerializeField]
    float explosionRadius = 10f;
    [SerializeField]
    float upwardsModifier = 250f;

    [Header("Lifetime")]
    [SerializeField]
    float lifetime = 1f;

    void Start() {
        gun.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
        man.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
        Destroy(this.gameObject, lifetime);
    }

    void DealDamage(Weapon weapon) {
        return;
	}
}
