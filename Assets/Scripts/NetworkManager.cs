using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NicknameInput;
    public GameObject DisconnectPanel;
    public GameObject LoadingPanel;
    public GameObject SpawnPanel;
    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public bool isHost;
    public bool hasVictoryDetermined;
    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        isHost = false;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        isHost = false;
        DisconnectPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.JoinRandomRoom();
        hasVictoryDetermined = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("Room", new RoomOptions { MaxPlayers = 2 });
        isHost = true;
        LoadingPanel.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            DisconnectPanel.SetActive(false);
            SpawnPanel.SetActive(true);
            SpawnCastle();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            LoadingPanel.SetActive(false);
            SpawnPanel.SetActive(true);
            SpawnCastle();
        }
    }
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        // {
        //     PhotonNetwork.Disconnect();
        // }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        SpawnPanel.SetActive(false);
        hasVictoryDetermined = false;
    }

    public void OnClickCancel()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SpawnPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        DisconnectPanel.SetActive(true);
        VictoryPanel.SetActive(false);
        DefeatPanel.SetActive(false);
    }

    public void SpawnCastle()
    {
        Vector3 spawnPos = isHost ? new Vector3(-8, -3.5f, 0) : new Vector3(8, -3.5f, 0);
        string castleType = isHost ? "BaseBlue" : "BaseBlue";
        PhotonNetwork.Instantiate(castleType, spawnPos, Quaternion.identity);
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}