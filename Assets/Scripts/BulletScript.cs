using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletScript : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    int dir;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.5f);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if(!PV.IsMine && collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine)
        {
            collision.GetComponent<PlayerScript>().Hit();
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DirRPC(int dir)
    {
        this.dir = dir;
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
