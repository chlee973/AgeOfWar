using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Unit : MonoBehaviourPunCallbacks, IPunObservable
{

    private Rigidbody2D rigidBody;
    private CapsuleCollider2D capsuleCollider;
    private PhotonView photonView;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private RaycastHit2D enemyObj;
    public Image healthImage;

    [SerializeField] private float idleDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    //private Unit target;

    private float cooldownTimer = Mathf.Infinity;

    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private bool isRed;

    private bool isHost;
    private LayerMask enemyLayer;
    private LayerMask allyLayer;
    Vector3 curPos;
    void Awake()
    {
        Debug.Log("유닛 생성");
        isHost = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isHost;
        rigidBody = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        currentHealth = maxHealth;
        if (photonView.IsMine)
        {
            photonView.RPC("FlipXRPC", RpcTarget.All, isHost);
            photonView.RPC("LayerRPC", RpcTarget.All, isHost);
        }
        UpdateHealth();
    }
    

    private void Update()
    {
        if(photonView.IsMine)
        {
            cooldownTimer += Time.deltaTime;
            if (EnemyInSight())
            {
                Stop();
                if(cooldownTimer >= Random.Range((float)(attackCooldown - 0.2f), (float)(attackCooldown + 3.0f)))
                // if (cooldownTimer >= attackCooldown)
                {
                    cooldownTimer = 0;
                    photonView.RPC("AttackRPC", RpcTarget.All);
                    enemyObj.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                }

            }
            else if (AllyInSight())
            {
                Stop();
            }
            else
            {
                Move();
            }
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    private void Stop()
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        animator.SetBool("idle", true);
    }

    private void Move()
    {
        float horizontal = isHost ? moveSpeed : -moveSpeed;
        rigidBody.velocity = new Vector2(horizontal, rigidBody.velocity.y);
        animator.SetBool("idle", false);
    }

    private bool EnemyInSight()
    {
        Vector2 dir = isHost ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)capsuleCollider.bounds.center + dir * capsuleCollider.size.x, dir, attackDistance, enemyLayer);
        Debug.DrawRay((Vector2)capsuleCollider.bounds.center + dir * capsuleCollider.size.x, dir * attackDistance, Color.red);
        bool result = hit.collider != null;
        if(result == true)
        {
            enemyObj = hit;
        }
        return result;
    }
    private bool AllyInSight()
    {
        Vector2 dir = isHost ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)capsuleCollider.bounds.center + dir * capsuleCollider.size.x, dir, idleDistance, allyLayer);
        Debug.DrawRay((Vector2)capsuleCollider.bounds.center + dir * capsuleCollider.size.x, dir * idleDistance, Color.blue);
        return hit.collider != null;
    }

    
    [PunRPC]
    private void AttackRPC()
    {
        animator.SetTrigger("attack");
    }
    [PunRPC]
    private void StopRPC()
    {
        animator.SetBool("idle", true);
    }
    [PunRPC]
    private void MoveRPC()
    {
        animator.SetBool("idle", false);
    }

    [PunRPC]
    void DestroyRPC()
    {
        float time;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;    //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == "Die")        //If it has the same name as your clip
            {
                time = ac.animationClips[i].length;
                animator.SetTrigger("die");
                Destroy(gameObject, time);
            }
        }
    }

    [PunRPC]
    void FlipXRPC(bool isHost)
    {
        spriteRenderer.flipX = !isHost;
    }
    [PunRPC]
    void LayerRPC(bool isHost)
    {
        gameObject.layer = isHost ? LayerMask.NameToLayer("Red") : LayerMask.NameToLayer("Blue");
        allyLayer = 1 << gameObject.layer;
        enemyLayer = 1 << (isHost ? LayerMask.NameToLayer("Blue") : LayerMask.NameToLayer("Red"));
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
            Debug.Log("딜 들어가는중");
            currentHealth -= damage;
            if(currentHealth <= 0)
            {
                photonView.RPC("DestroyRPC", RpcTarget.All);
                
            }
            else
            {
                UpdateHealth();
            }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(currentHealth);
                stream.SendNext(healthImage.fillAmount);
            }
            else
            {
                curPos = (Vector3)stream.ReceiveNext();
                currentHealth = (float)stream.ReceiveNext();
                healthImage.fillAmount = (float)stream.ReceiveNext();
            }
    }

    public void UpdateHealth()
    {
        healthImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
