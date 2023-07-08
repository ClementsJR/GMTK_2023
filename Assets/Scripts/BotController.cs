using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour {
    [SerializeField]
    Transform[] opponents;
    Transform currentTarget;

    [Header("Movement")]
    [SerializeField][Range(0f, 50f)]
    float moveSpeed = 10f;
    [SerializeField][Range(0f,5f)]
    float turnSpeed = 3f;
    /*[SerializeField][Range(0f, 5000f)]
    float jumpForce = 1000f;*/
    [SerializeField]
    Rigidbody physicsBody;

    [Header("Weapon")]
    [SerializeField]
    Weapon gun;
    [SerializeField][Range(0f, 1f)]
    float fireRate = 0.3f;
    [SerializeField][Range(0f, 500f)]
    float maxFireDistance = 100f;
    float lastFireTime;

	private void Start() {
        currentTarget = opponents[0];
        lastFireTime = 0f;
	}

	private void FixedUpdate() {
        float distanceTo = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceTo > 30f) ApproachTarget();
        if (distanceTo < 20f) PullAwayFromTarget();
    }

	void Update() {
        currentTarget = AcquireTarget();
        FaceTarget();

        lastFireTime += Time.deltaTime;
        if(lastFireTime > fireRate)
            CheckFire();
    }

    Transform AcquireTarget() {
        Transform target = currentTarget;
        float bestDistance = Vector3.Distance(transform.position, target.position);

        foreach (Transform opponent in opponents) {
            float distance = Vector3.Distance(transform.position, opponent.position);
            if (distance < bestDistance) {
                target = opponent;
                bestDistance = distance;
			}
		}

        float currentDistance = Vector3.Distance(transform.position, currentTarget.position);
        if (currentDistance - bestDistance <= bestDistance)
            target = currentTarget;

        return target;
	}

    void FaceTarget() {
        Quaternion rotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }

    void ApproachTarget() {
        Vector3 movement = Vector3.forward * moveSpeed;// * Time.fixedDeltaTime;
        movement = transform.TransformDirection(movement);
        movement.y = physicsBody.velocity.y;
        physicsBody.velocity = movement;
    }

    void PullAwayFromTarget() {
        Vector3 movement = -Vector3.forward * moveSpeed;// * Time.fixedDeltaTime;
        movement = transform.TransformDirection(movement);
        movement.y = physicsBody.velocity.y;
        physicsBody.velocity = movement;
    }

    void CheckFire() {
        Vector3 targetDir = transform.forward;
        bool targetVisible = Physics.Raycast(transform.position, targetDir, out RaycastHit hit);
        Debug.DrawRay(transform.position, targetDir);

        if (targetVisible && hit.distance < maxFireDistance && hit.collider.transform == currentTarget) {
            gun.Fire(targetDir);
            lastFireTime = 0f;
        }
    }
}
