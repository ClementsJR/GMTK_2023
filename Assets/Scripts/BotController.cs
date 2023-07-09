using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class BotController : GenericController {
    
    [Header("Bot Options")]
    [SerializeField][Range(0f, 500f)]
    float maxFireDistance = 100f;
    [SerializeField][Range(0f, 1f)]
    float patrolChance = 0.1f;
    [SerializeField]
    float maxPatrolDistance = 50f;
    [SerializeField]
    float fireAngleLimit = 5f;
    [SerializeField]
    float switchFromFightTime = 3f;
    [SerializeField]
    float startRetreatLimit = 0.25f;
    [SerializeField]
    float endRetreatLimit = 0.5f;

    /*[Header("Debug")]
    public TextMeshProUGUI onScreenLog;*/

    List<Transform> enemies;
    List<Transform> visibleEnemies;
    NavMeshPath patrolPath;
    Vector3 currentPatrolCorner = Vector3.zero;
    int nextPatrolCornerIndex = 0;
    float patrolCutoffDistance = 0.5f;
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
    }

	private void OnEnable() {
        if (patrolPath == null)
            patrolPath = new NavMeshPath();

        currentTarget = null;
        timeSinceEnemySeen = 0f;

        SwitchToPatrol();
	}

	private void FixedUpdate() {
        UpdateVisibleEnemies();

        if (state == BotState.Patrol) {
            MoveAlongPatrol();
		}

        if (state == BotState.Fight) {
            float distanceTo = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceTo > 30f) ApproachTarget();
            if (distanceTo < 10f) PullAwayFromTarget();
        }

        if (state == BotState.Retreat) {
            PullAwayFromTarget();
		}
    }

	void Update() {
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
        }
    }

    void SwitchToPatrol() {
        state = BotState.Patrol;

        Vector3 randomDir = Random.insideUnitSphere * maxPatrolDistance;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit foundPoint, maxPatrolDistance, 1)) {
            Vector3 patrolTarget = foundPoint.position;
            NavMesh.CalculatePath(transform.position, patrolTarget, 1, patrolPath);
            currentPatrolCorner = patrolPath.corners[0];
            nextPatrolCornerIndex = 1;
        } else
            SwitchToCamp();
    }

	private void OnDrawGizmos() {
        if (state == BotState.Patrol) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, currentPatrolCorner);
            Gizmos.DrawSphere(currentPatrolCorner, 0.5f);

            Gizmos.color = Color.green;
            Vector3 cornerDirection = DirectionTo(currentPatrolCorner);
            cornerDirection.y = 0f;
            cornerDirection.Normalize();
            Gizmos.DrawRay(transform.position, cornerDirection);
        } else if (state == BotState.Fight) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }

	void MoveAlongPatrol() {
        float distanceNeeded = Vector3.Distance(transform.position, currentPatrolCorner);

        if (distanceNeeded < patrolCutoffDistance) {
            if (nextPatrolCornerIndex >= patrolPath.corners.Length) {
                SwitchToCamp();
                return;
            } else {
                currentPatrolCorner = patrolPath.corners[nextPatrolCornerIndex];
                nextPatrolCornerIndex++;
            }
		}

        Face(currentPatrolCorner);
        Vector3 cornerDirection = DirectionTo(currentPatrolCorner);
        cornerDirection.y = 0f;
        cornerDirection.Normalize();
        float angle = Vector3.Angle(transform.forward, cornerDirection);
        if (angle < 1f)
            ApproachTarget();
	}

    void HandleFight() {
        if (healthSystem.HealthPercent() < startRetreatLimit) {
            state = BotState.Retreat;
        } else if (visibleEnemies.Contains(currentTarget)) {
            Face(currentTarget.position);
            if (gun.CanFire()) {
                CheckFire();
            }
        } else if (visibleEnemies.Count > 0) {
            currentTarget = NearestEnemy();
            Face(currentTarget.position);
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

    void Face(Vector3 target) {
        Quaternion rotation = Quaternion.LookRotation(target - transform.position);
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
        return DirectionTo(enemy.position);
    }

    Vector3 DirectionTo(Vector3 position) {
        return transform.TransformDirection(transform.InverseTransformPoint(position));
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

    void DealDamage(Weapon weapon) {
        if (state == BotState.Fight || state == BotState.Retreat)
            return;

        if (visibleEnemies.Count > 0) {
            SwitchToFight();
            if (visibleEnemies.Contains(weapon.Attacker()))
                currentTarget = weapon.Attacker();
		} else {
            SwitchToRetreat();
		}
    }
}
