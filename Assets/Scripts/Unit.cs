using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Unit : MonoBehaviourPunCallbacks, IPunObservable
{

    private Rigidbody2D rigidBody;
    private CapsuleCollider2D capsuleCollider;
    private PhotonView photonView;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private GameObject enemyObj;

    [SerializeField] private LayerMask idleLayer;
    [SerializeField] private LayerMask attackLayer;

    [SerializeField] private float idleDistance;
    [SerializeField] private float idleRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    //private Unit target;

    private float cooldownTimer = Mathf.Infinity;

    private float currentHealth;
    [SerializeField] private float maxHealth;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        cooldownTimer += Time.deltaTime;
        if (EnemyInSight())
        {
            Stop();
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                Attack();
                animator.SetTrigger("attack");
            }

        }
        else if (AllyInSight())
        {
            Debug.Log("아군 앞에 있음");
            Stop();
        }
        else
        {
            Move();
        }
    }

    private void Stop()
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        animator.SetBool("idle", true);
    }

    private void Move()
    {
        rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
        animator.SetBool("idle", false);
    }

    private bool EnemyInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)capsuleCollider.bounds.center + Vector2.right * capsuleCollider.size.x, Vector2.right, attackDistance, attackLayer);
        Debug.DrawRay((Vector2)capsuleCollider.bounds.center + Vector2.right * capsuleCollider.size.x, Vector3.right * attackDistance, Color.red);
        bool result = hit.collider != null;
        if(result == true)
        {
            enemyObj = hit.collider.gameObject;
        }
        return result;
    }
    private bool AllyInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)capsuleCollider.bounds.center + Vector2.right * capsuleCollider.size.x, Vector2.right, attackDistance, idleLayer);
        Debug.DrawRay((Vector2)capsuleCollider.bounds.center + Vector2.right * capsuleCollider.size.x, Vector3.right * idleDistance, Color.blue);
        return hit.collider != null;
    }

    

    private void Attack()
    {
        enemyObj.GetComponent<Unit>().TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        //animator.SetTrigger("")
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new NotImplementedException();
    }
}
