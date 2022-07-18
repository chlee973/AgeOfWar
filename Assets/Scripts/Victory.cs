using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class Victory : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image healthImage;
    private PhotonView PV;
    // Start is called before the first frame update
    // Update is called once per frame
    private void Awake() 
    {
        PV = gameObject.GetComponent<PhotonView>();
    }
    void OnDestroy()
    {
        if(!GameObject.Find("NetworkManager").GetComponent<NetworkManager>().hasVictoryDetermined)
        {
            if(PV.IsMine)
            {
                GameObject.Find("Canvas").transform.Find("Victory Panel").gameObject.SetActive(false);
                GameObject.Find("Canvas").transform.Find("Defeat Panel").gameObject.SetActive(true);
                GameObject.Find("NetworkManager").GetComponent<NetworkManager>().hasVictoryDetermined = true;
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("Victory Panel").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("Defeat Panel").gameObject.SetActive(false);
                GameObject.Find("NetworkManager").GetComponent<NetworkManager>().hasVictoryDetermined = true;
            }

        }
    }
}
