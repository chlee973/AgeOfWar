using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public void OnClickNinjaSpawn()

    {
        bool isHost = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isHost;
        Vector3 spawnPos = isHost ? new Vector3(-6, -3, 0) : new Vector3(6, -3, 0);
        PhotonNetwork.Instantiate("Ninja", spawnPos, Quaternion.identity);
    }

    public void OnClickWolfSpawn()
    {

        bool isHost = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isHost;
        Vector3 spawnPos = isHost ? new Vector3(-6, -3, 0) : new Vector3(6, -3, 0);
        PhotonNetwork.Instantiate("Wolf", spawnPos, Quaternion.identity);
    }
}
