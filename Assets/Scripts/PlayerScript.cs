using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D RB;
    public Animator AN;
    public SpriteRenderer SR;
    public PhotonView PV;
    public Text NicknameText;
    public Image HealthImage;
    bool isGround;
    Vector3 curPos;


    private void Awake()
    {
        NicknameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NicknameText.color = PV.IsMine ? Color.green : Color.red;
        if(PV.IsMine)
        {
            bool isHost = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isHost;
            PV.RPC("FlipXRPC", RpcTarget.AllBuffered, isHost ? 1.0f : -1.0f);
        }
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            float axis = Input.GetAxisRaw("Horizontal");
            RB.velocity = new Vector2(4 * axis, RB.velocity.y);
            if (axis != 0)
            {
                AN.SetBool("walk", true);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);
            }
            else
            {
                AN.SetBool("walk", false);
            }
            // 바닥 체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + (RB.gravityScale > 0 ? new Vector2(0, -0.5f) : new Vector2(0, 0.5f)), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
            AN.SetBool("jump", !isGround);
            if (Input.GetKeyDown(KeyCode.LeftShift) && isGround)
            {
                PV.RPC("JumpRPC", RpcTarget.AllBuffered);
            }
            PV.RPC("FlipYRPC", RpcTarget.AllBuffered, RB.gravityScale);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity)
                    .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
                PV.RPC("ShotRPC", RpcTarget.AllBuffered);
            }

        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    [PunRPC]
    void FlipXRPC(float axis) => SR.flipX = axis == -1;

    [PunRPC]
    void FlipYRPC(float gravityScale)
    {
        SR.flipY = gravityScale < 0;
    }

    [PunRPC]
    void JumpRPC()
    {
        RB.gravityScale = -RB.gravityScale;
    }

    public void Hit()
    {
        HealthImage.fillAmount -= 0.05f;
        if(HealthImage.fillAmount <= 0)
        {
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            if(PV.IsMine)
            {
                GameObject.Find("Canvas").transform.Find("Respawn Panel").gameObject.SetActive(true);
            }
        }
    }
    [PunRPC] 
    void ShotRPC()
    {
        AN.SetTrigger("shot");
    }
    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();  
        }
    }

}
