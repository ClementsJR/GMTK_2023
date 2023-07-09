using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnHandler : MonoBehaviour {

    [SerializeField]
    float spawnTime = 2f;
    [SerializeField]
    GenericController controller;
    [SerializeField]
    HealthSystem health;
    [SerializeField]
    Spawner spawner;

    [Header("UI")]
    [SerializeField]
    bool displayTimer = false;
    [SerializeField]
    GameObject respawnScreen;
    [SerializeField]
    TextMeshProUGUI timerLabel;

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

        if (displayTimer) {
            respawnScreen.SetActive(true);
            UpdateLabel();
        }
    }

    void Update() {
        timeToSpawn -= Time.deltaTime;
        if (displayTimer) UpdateLabel();

            if (timeToSpawn <= 0f) {
            Vector3 spawnLocation = spawner.GetSpawnLocation();
            transform.position = spawnLocation;

            health.enabled = true;
            controller.enabled = true;
            if (displayTimer)  respawnScreen.SetActive(false);
            this.enabled = false;
		}
    }

    void UpdateLabel() {
        timerLabel.text = timeToSpawn.ToString("F2");
	}
}
