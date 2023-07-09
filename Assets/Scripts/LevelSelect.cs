using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LevelSelect : MonoBehaviourPunCallbacks {

	public GameObject masterControls;

	public TextMeshProUGUI timer;
	public TextMeshProUGUI playerDisp;
	public TextMeshProUGUI botDisp;
	public TextMeshProUGUI levelDisp;

	//private PhotonView photonView;
	private string selectedLevel = "Level 1";
	private bool isMaster;
	private float startCountdown = 60f;
	private int playerCount;

	private void Start() {
		isMaster = PhotonNetwork.IsMasterClient;
		//photonView = PhotonView.Get(this);
		playerCount = 1;

		if (isMaster) DisplayMasterControls();
	}

	private void Update() {
		startCountdown -= Time.deltaTime;
		UpdateTimer();

		if (isMaster) {
			photonView.RPC("UpdateTimer", RpcTarget.Others, startCountdown);

			if (startCountdown <= 0f) StartMatch();
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer) {
		playerCount++;
		UpdatePlayerDisp();

		if (isMaster) {
			photonView.RPC("SyncNewPlayer", newPlayer, startCountdown, playerCount, selectedLevel);
			if (playerCount == 5) {
				startCountdown = 3f;
				photonView.RPC("AdvanceTimer", RpcTarget.Others, startCountdown);
			}
		}
	}

	public override void OnPlayerLeftRoom(Player newPlayer) {
		playerCount--;
		UpdatePlayerDisp();
	}

	void DisplayMasterControls() {
		masterControls.SetActive(true);
	}

	public void SelectLevel(string level) {
		selectedLevel = level;
		photonView.RPC("UpdateLevel", RpcTarget.Others, selectedLevel);
	}

	public void StartMatch() {
		PhotonNetwork.LoadLevel(selectedLevel);
	}

	[PunRPC]
	void SyncNewPlayer(float timeLeft, int currPlayerCount, string level) {
		startCountdown = timeLeft;
		UpdateTimer();

		playerCount = currPlayerCount;
		UpdatePlayerDisp();

		UpdateLevel(level);
	}

	[PunRPC]
	void UpdateLevel(string levelName) {
		levelDisp.text = "Selected Level:\n" + levelName;
	}

	[PunRPC]
	void AdvanceTimer(float time) {
		startCountdown = time;
		UpdateTimer();
	}

	void UpdateTimer() {
		timer.text = startCountdown.ToString("F0");
	}

	void UpdatePlayerDisp() {
		playerDisp.text = playerCount + " players in lobby";
		botDisp.text = (5 - playerCount) + " bots will join in";
	}
}
