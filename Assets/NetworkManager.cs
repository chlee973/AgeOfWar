using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NicknameInput;
    public GameObject MenuSet;
    public GameObject LoadingSet;
    public GameObject BaseBlue;
    public GameObject BaseRed;
    public GameObject GameSet;

    void Awake()
    {
        Screen.SetResolution(285, 135, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        MenuSet.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("Room", new RoomOptions { MaxPlayers = 2 });
        LoadingSet.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            MenuSet.SetActive(false);
            GameSet.SetActive(true);
            BaseBlue.SetActive(true);
            BaseRed.SetActive(true);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        LoadingSet.SetActive(false);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            GameSet.SetActive(true);
            BaseBlue.SetActive(true);
            BaseRed.SetActive(true);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        MenuSet.SetActive(true);
        LoadingSet.SetActive(false);

        GameSet.SetActive(false);
        BaseBlue.SetActive(false);
        BaseRed.SetActive(false);
    }

    public void OnClickCancelBtn()
    {
        PhotonNetwork.Disconnect();
        MenuSet.SetActive(true);
        LoadingSet.SetActive(false);

        GameSet.SetActive(false);
        BaseBlue.SetActive(false);
        BaseRed.SetActive(false);
    }


}