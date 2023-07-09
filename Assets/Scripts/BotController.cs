using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BotController : GenericController {
    
    [Header("Bot Options")]
    [SerializeField][Range(0f, 500f)]
    float maxFireDistance = 100f;
    [SerializeField][Range(0f, 1f)]
    float patrolChance = 0.1f;
    [SerializeField][Range(0f, 1f)]
    float campChance = 0.01f;
    [SerializeField]
    float fireAngleLimit = 5f;
    [SerializeField]
    float switchFromFightTime = 3f;
    [SerializeField]
    float startRetreatLimit = 0.25f;
    [SerializeField]
    float endRetreatLimit = 0.5f;

    [Header("Debug")]
    public TextMeshProUGUI onScreenLog;

    List<Transform> enemies;
    List<Transform> visibleEnemies;
    Transform currentTarget;
    BotState state;
    float timeSinceEnemySeen;

    private void Start() {
        enemies = new List<Transform>();
        visibleEnemies = new List<Transform>();

        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject character in characters) {
            if (this.gameObject != character)
                enemies.Add(character.transform);
		}

        currentTarget = null;
	}

	private void OnEnable() {
        SwitchToPatrol();
        timeSinceEnemySeen = 0f;
	}

	private void FixedUpdate() {
        UpdateVisibleEnemies();

        if(state == BotState.Fight) {
            float distanceTo = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceTo > 30f) ApproachTarget();
            if (distanceTo < 10f) PullAwayFromTarget();
        }
    }

	void Update() {
        onScreenLog.text = "" + state;

        switch(state) {
        case BotState.Camp:
            HandleCamp();
            break;
        case BotState.Patrol:
            HandlePatrol();
            break;
        case BotState.Fight:
            HandleFight();
            break;
        case BotState.Retreat:
            HandleRetreat();
            break;
		}
    }

    void HandleCamp() {
        if (visibleEnemies.Count > 0) {
            SwitchToFight();
		} else {
            float patrolRoll = Random.Range(0f, 1f);
            if (patrolRoll <= patrolChance)
                SwitchToPatrol();
        }
    }

    void SwitchToCamp() {
        state = BotState.Camp;
    }

    void HandlePatrol() {
        if (visibleEnemies.Count > 0) {
            SwitchToFight();
        } else {
            float campRoll = Random.Range(0f, 1f);
            if (campRoll <= campChance)
                SwitchToCamp();
		}

        //Move towards patrol point
    }

    void SwitchToPatrol() {
        state = BotState.Patrol;


    }

    void HandleFight() {
        if (healthSystem.HealthPercent() < startRetreatLimit) {
            state = BotState.Retreat;
        } else if (visibleEnemies.Contains(currentTarget)) {
            FaceTarget();
            if (gun.CanFire()) {
                CheckFire();
            }
        } else if (visibleEnemies.Count > 0) {
            currentTarget = NearestEnemy();
            FaceTarget();
            if (gun.CanFire())
                CheckFire();
        } else if (timeSinceEnemySeen > switchFromFightTime) {
            state = BotState.Camp;
            timeSinceEnemySeen = 0f;
        } else {
            timeSinceEnemySeen += Time.deltaTime;
		}
    }

    void SwitchToFight() {
        state = BotState.Fight;
        currentTarget = NearestEnemy();
    }

    void HandleRetreat() {
        if (healthSystem.HealthPercent() > endRetreatLimit) {
            SwitchToCamp();
		}
    }

    void SwitchToRetreat() {
        state = BotState.Retreat;
    }

    Transform AcquireTarget() {
        Transform bestTarget = currentTarget;
        float bestDistance = Vector3.Distance(transform.position, bestTarget.position);

        foreach (Transform opponent in enemies) {
            float distance = Vector3.Distance(transform.position, opponent.position);
            if (distance < bestDistance) {
                bestTarget = opponent;
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

    void StrafeTarget() {

	}

    Vector3 DirectionTo(Transform enemy) {
        return transform.TransformDirection(transform.InverseTransformPoint(enemy.position));
    }

    void CheckFire() {
        Vector3 targetDir = DirectionTo(currentTarget);

        float angleFromForward = Vector3.Angle(transform.forward, targetDir);
        if (angleFromForward > fireAngleLimit) return;

        bool targetVisible = Physics.Raycast(transform.position, targetDir, out RaycastHit hit);
        if (targetVisible && hit.distance < maxFireDistance && hit.collider.transform == currentTarget) {
            gun.Fire(targetDir);
        }
    }

    void UpdateVisibleEnemies() {
        visibleEnemies.Clear();

        foreach(Transform enemy in enemies) {
            Vector3 direction = DirectionTo(enemy);
            if(Physics.Raycast(transform.position, direction, out RaycastHit hit) && hit.transform == enemy) {
                visibleEnemies.Add(enemy);
			}
		}
    }

    Transform NearestEnemy(bool visibleOnly = true) {
        List<Transform> enemyList = visibleOnly ? visibleEnemies : enemies;

        Transform closestEnemy = enemyList[0];
        float closestDistance = Vector3.Distance(transform.position, closestEnemy.position);

        foreach (Transform enemy in enemyList) {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < closestDistance) {
                closestEnemy = enemy;
                closestDistance = distance;
			}
		}

        return closestEnemy;
	}

    void DealDamage(float damage, Transform attacker) {
        if (state == BotState.Fight || state == BotState.Retreat)
            return;

        if (visibleEnemies.Count > 0) {
            SwitchToFight();
            if (visibleEnemies.Contains(attacker))
                currentTarget = attacker;
		} else {
            SwitchToRetreat();
		}
    }
}
