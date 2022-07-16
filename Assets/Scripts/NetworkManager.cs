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
    public GameObject RespawnPanel;
    public GameObject LoadingPanel;
    public bool isHost;

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
        DisconnectPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.JoinRandomRoom();
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
            //StartCoroutine("DestroyBullet");
            Spawn();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            LoadingPanel.SetActive(false);
            Spawn();
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
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }

    //IEnumerator DestroyBullet()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    foreach (GameObject Go in GameObject.FindGameObjectsWithTag("Bullet")) Go.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
    //}
    public void Spawn()
    {
        Debug.Log("스폰 실행됨");
        PhotonNetwork.Instantiate("Player", isHost ? new Vector3(-6, 0, 0) : new Vector3(6, 0, 0), Quaternion.identity);
        RespawnPanel.SetActive(false);
    }

    public void OnClickCancel()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        LoadingPanel.SetActive(false);
        DisconnectPanel.SetActive(true);
    }

}