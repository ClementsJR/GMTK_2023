using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuController : MonoBehaviourPunCallbacks {

	const string gameVersion = "0.1";

	public Button playBtn;
	public GameObject connectingMessage;

	private byte maxPlayersPerRoom = 5;
	private string sceneToLoad;

    public void PlayGame(string firstScene) {
		sceneToLoad = firstScene;

		playBtn.enabled = false;
		connectingMessage.SetActive(true);

		Connect();
	}

	public void Connect() {
		if (PhotonNetwork.IsConnected) {
			PhotonNetwork.JoinRandomRoom();
		} else {
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = gameVersion;
		}
	}

	public override void OnJoinRandomFailed(short returnCode, string message) {
		Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
	}

	public override void OnJoinedRoom() {
		Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
		PhotonNetwork.LoadLevel(sceneToLoad);
	}

	public override void OnConnectedToMaster() {
		Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
	}

	public override void OnDisconnected(DisconnectCause cause) {
		Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
	}
}
