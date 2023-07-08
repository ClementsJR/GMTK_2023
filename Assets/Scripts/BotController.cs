using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : GenericController {
    
    [Header("Bot Options")]
    [SerializeField][Range(0f, 500f)]
    float maxFireDistance = 100f;

    List<Transform> targets;
    Transform currentTarget;
    BotState state;

    private void Start() {
        targets = new List<Transform>();

        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject character in characters) {
            if (this.gameObject != character)
                targets.Add(character.transform);
		}
        currentTarget = targets[0];
	}

	private void OnEnable() {
        state = BotState.Patrol;
	}

	private void FixedUpdate() {
        float distanceTo = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceTo > 30f) ApproachTarget();
        if (distanceTo < 20f) PullAwayFromTarget();
    }

	void Update() {
        currentTarget = AcquireTarget();
        FaceTarget();
        if (gun.CanFire())
            CheckFire();
    }

    Transform AcquireTarget() {
        Transform bestTarget = currentTarget;
        float bestDistance = Vector3.Distance(transform.position, bestTarget.position);

        foreach (Transform target in targets) {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < bestDistance) {
                bestTarget = target;
                bestDistance = distance;
			}
		}

        float currentDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (currentDistance - bestDistance <= bestDistance)
            bestTarget = currentTarget;

        return bestTarget;
	}

    void FaceTarget() {
        Quaternion rotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookSpeed);
    }

    void ApproachTarget() {
        Vector3 movement = Vector3.forward * moveSpeed;
        movement = transform.TransformDirection(movement);
        movement.y = physicsBody.velocity.y;
        physicsBody.velocity = movement;
    }

    void PullAwayFromTarget() {
        Vector3 movement = -Vector3.forward * moveSpeed;
        movement = transform.TransformDirection(movement);
        movement.y = physicsBody.velocity.y;
        physicsBody.velocity = movement;
    }

    void CheckFire() {
        Vector3 targetDir = transform.forward;
        bool targetVisible = Physics.Raycast(transform.position, targetDir, out RaycastHit hit);

        if (targetVisible && hit.distance < maxFireDistance && hit.collider.transform == currentTarget) {
            gun.Fire(targetDir);
        }
    }

    //GenericController
}
