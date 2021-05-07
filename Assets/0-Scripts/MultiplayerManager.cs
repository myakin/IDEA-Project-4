using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerManager : MonoBehaviourPunCallbacks {
    public static MultiplayerManager instance;

    private void Awake() {
        if (instance==null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance!=this) {
            Destroy(gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public string[] playerPrefabNames;
    public Transform[] spawnAreas;
    public GameObject lobbyCamera;

    private void Start() {
        if (UIManager.instance == null) {
            AddressablesManager.instance.LoadAddressableSceneAdditive("MainPlayerUI", delegate { ConnectToServer(); });
        } else {
            ConnectToServer();
        }
    }

    public void ConnectToServer() {
        // do PUN things
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster() {
        // change UI
         if (UIManager.instance == null) {
            AddressablesManager.instance.LoadAddressableSceneAdditive(
                "MainPlayerUI", 
                delegate { 
                    UIManager.instance.SetStatus(PhotonNetwork.IsConnected);
                    PhotonNetwork.JoinLobby(); 
                }
            );
        } else {
            UIManager.instance.SetStatus(PhotonNetwork.IsConnected);

            // join lobby
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnJoinedLobby() {
        Debug.Log("Joined lobby");
        // TODO: list available rooms

        JoinRoom();
    }

    public void JoinRoom() {
        RoomOptions roomOptions = new RoomOptions();
        TypedLobby lobby = PhotonNetwork.CurrentLobby;
        PhotonNetwork.JoinOrCreateRoom("MyFirstRoom", roomOptions, lobby);
    }
    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
        GameManager.instance.SpawnWorld();
    }
    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("Joined room failed: "+returnCode+" "+message);
    }

    public void SpawnPlayer() {
        int playerPrefabIndex = Random.Range(0, playerPrefabNames.Length);
        int spawnAreaIndex = Random.Range(0, spawnAreas.Length);


        Vector3 spawnPos = spawnAreas[spawnAreaIndex].transform.position + Quaternion.Euler(0, Random.Range(0, 360), 0) * (spawnAreas[spawnAreaIndex].transform.forward * 20);
        Quaternion spawnRot = Quaternion.identity;

        GameObject spawnedPlayer = PhotonNetwork.Instantiate(playerPrefabNames[playerPrefabIndex], spawnPos, spawnRot);
        lobbyCamera.SetActive(false);
        spawnedPlayer.GetComponent<Rigidbody>().isKinematic = false;
    }






}
