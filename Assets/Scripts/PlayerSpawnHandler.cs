using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour {

    [SerializeField]
    float spawnTime = 2f;
    [SerializeField]
    PlayerController player;
    [SerializeField]
    Transform waitArea;
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
        
        player.transform.position = waitArea.position;
    }

    void Update() {
        timeToSpawn -= Time.deltaTime;

        if (timeToSpawn <= 0f) {
            Vector3 spawnLocation = spawner.GetSpawnLocation();
            player.transform.position = spawnLocation;
            player.enabled = true;
            this.enabled = false;
		}
    }

	
}
