using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private LayerMask allylayer;
    [SerializeField] private LayerMask enemylayer;

    [SerializeField] private float speed;
    [SerializeField] private float hp;
    [SerializeField] private float atkdamage;
    [SerializeField] private float atkrange;
    [SerializeField] private float atkcooldown;

    private float cooldowntimer = Mathf.Infinity;

    private GameObject scanobj;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("doMove", true);
    }

    private void Update()
    {
        animator.SetBool("doMove", true);
        transform.position += Vector3.right * speed * Time.deltaTime;
        //cooldowntimer += Time.deltaTime;
        //if (ScanEnemy())
        //{
        //    if (cooldowntimer >= atkcooldown)
        //    {
        //        cooldowntimer = 0;
        //        animator.SetTrigger("doAttack");
        //        scanobj.GetComponent<Unit>().Hit(atkdamage);
        //        //scanobj.Hit(Atk);
        //    }
        //    // 어떻게하지? 랜덤 공속마다 하고 다시 Move.
        //    // Hit 해서 ScanObj에서 Atk 빼야함.
        //}
        //else if (ScanAlly())
        //{
        //    animator.SetTrigger("doStop");
        //}
        //else
        //{
        //    animator.SetTrigger("doMove");
        //    transform.position += Vector3.right * speed * Time.deltaTime;
        //}


    }

    private bool ScanEnemy()
    {
        RaycastHit2D rh = Physics2D.Raycast(transform.position, Vector2.right, 0.7f, enemylayer);
        if (rh.collider != null)
        {
            scanobj = rh.collider.gameObject; // 감지한 상대 저장
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ScanAlly()
    {
        RaycastHit2D rh = Physics2D.Raycast(transform.position, Vector2.right, 0.3f, allylayer);
        return rh.collider != null;
    }

    public void Hit(float damage)
    {
        hp = hp - damage;
        if (hp <= 0)
        {
            // Die();
        }
        else
        {
            animator.SetTrigger("doHit");
        }
    }

    private void Die()
    {
        animator.SetTrigger("doDie");
        // collider 끄기
        // Sprite renderer set sorting order를 1로 하기

        // Destory 해야함
    }
}
