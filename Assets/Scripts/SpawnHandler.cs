using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour {

    [SerializeField]
    float spawnTime = 2f;
    [SerializeField]
    GenericController controller;
    [SerializeField]
    HealthSystem health;
    [SerializeField]
    Spawner spawner;

    private float timeToSpawn;
    private bool isFirstSpawn = true;

    void OnEnable() {
        if(isFirstSpawn) {
            timeToSpawn = 0f;
            isFirstSpawn = false;
		} else {
            timeToSpawn = spawnTime;
        }
        
        controller.transform.position = spawner.GetWaitLocation();
    }

    void Update() {
        timeToSpawn -= Time.deltaTime;

        if (timeToSpawn <= 0f) {
            Vector3 spawnLocation = spawner.GetSpawnLocation();
            transform.position = spawnLocation;

            health.enabled = true;
            controller.enabled = true;
            this.enabled = false;
		}
    }
}
