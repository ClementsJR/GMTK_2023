using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RoundController : MonoBehaviour {

    [SerializeField]
    float startRoundTime = 60f * 3f;
    [SerializeField]
    float startExitTime = 15f;
    [SerializeField]
    TextMeshProUGUI timeDisplay;
    [SerializeField]
    GameObject roundEndPanel;
    [SerializeField]
    TMP_InputField nameDisplay;
    [SerializeField]
    TMP_InputField killDisplay;
    [SerializeField]
    TMP_InputField deathDisplay;
    [SerializeField]
    TMP_InputField ratioDisplay;
    [SerializeField]
    TextMeshProUGUI exitDisplay;

    float currentRoundTime = 0f;
    float timeToExit = 0f;
    bool roundIsOver = false;
    GenericController[] controllers;
    PlayerController player;
    Hashtable kills;
    Hashtable deaths;

    void Start() {
        currentRoundTime = startRoundTime;
        timeToExit = startExitTime;

        controllers = GameObject.FindObjectsOfType<GenericController>();
        player = GameObject.FindObjectOfType<PlayerController>();

        kills = new Hashtable();
        deaths = new Hashtable();
        foreach(GenericController controller in controllers) {
            string key = controller.name;
            kills.Add(key, 0);
            deaths.Add(key, 0);
		}
    }

    void Update() {
        if (!roundIsOver) {
            currentRoundTime -= Time.deltaTime;
            timeDisplay.text = currentRoundTime.ToString("F1");

            if (currentRoundTime <= 0f) {
                EndRound();
            }
        } else {
            timeToExit -= Time.deltaTime;
            exitDisplay.text = "Quitting in:\t" + timeToExit.ToString("F0");

            if (timeToExit <= 0f) {
                ExitStage();
			}
		}
    }

    void EndRound() {
        roundIsOver = true;

        foreach (GenericController controller in controllers) {
            controller.enabled = false;
        }
        roundEndPanel.SetActive(true);

        foreach (GenericController controller in controllers) {
            string name = controller.name;
            nameDisplay.text += name + "\n";

            int numKills = (int)(kills[name]);
            killDisplay.text += kills + "\n";

            int numDeaths = (int)(deaths[name]);
            deathDisplay.text += deaths + "\n";

            float kdRatio = numKills / (float)numDeaths;
            ratioDisplay.text += kdRatio.ToString("F1") + "\n";
		}
    }

    private void ExitStage() {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Level Select");
	}

    public void AwardKill(GenericController killer, GenericController victim) {
        kills[killer.name] = 1 + (int)kills[killer.name];
        deaths[victim.name] = 1 + (int)deaths[victim.name];
    }
}
