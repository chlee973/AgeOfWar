using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Nickname : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    [SerializeField] private Text nicknameText;
    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        nicknameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        nicknameText.color = PV.IsMine ? Color.green : Color.red;
    }
}
