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
    [SerializeField][Range(0f, 5000f)]
    float jumpForce = 1000f;

    [Header("Weapon")]
    [SerializeField]
    Weapon gun;

	private void Start() {
        currentTarget = opponents[0];
	}

	void Update() {
        currentTarget = AcquireTarget();

        float angleTo = Vector3.SignedAngle(transform.position, currentTarget.position, Vector3.up);
        if (angleTo > 0.25f) FaceTarget(angleTo);

        float distanceTo = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceTo > 30f) ApproachTarget();
        if (distanceTo < 20f) PullAwayFromTarget();
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

    void FaceTarget(float angle) {
        float rotation = Mathf.Sign(angle) * turnSpeed;
        transform.eulerAngles += Vector3.up * rotation;
    }

    void ApproachTarget() {
        Vector3 movement = Vector3.forward * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);
    }

    void PullAwayFromTarget() {
        Vector3 movement = -Vector3.forward * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);
    }
}
