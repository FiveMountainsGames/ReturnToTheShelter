using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatEnemy : MonoBehaviour
{
    public int hp = 20;
    private float speed = 10f;

    private Rigidbody2D rb;
    private GameObject player;
    private PlayerController pc;
    private bool isHitPlayer = false;
    private bool isSetDamage = false;
    private Transform playerLeg;
    private Animator anim;


    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        playerLeg = GameObject.FindGameObjectWithTag("Player_Leg_F").transform;
        anim = GetComponent<Animator>();

        if (transform.parent != null)
        {
            transform.parent = null;
        }
        pc.viewEnemy = this.gameObject;
        transform.localPosition = new Vector3(11.0f, Random.Range(0, 2), 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        anim.SetBool("isDead", false);
        transform.GetComponent<Collider2D>().enabled = true;
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(this.gameObject.transform.position, player.transform.position);
        Vector3 defaultDirection = transform.position + Vector3.left * speed * Time.fixedDeltaTime * GameManager.Instance.generalSpeed;
        if (!isHitPlayer && hp > 0)
        {
            if (distanceToPlayer > 8)
            {
                rb.MovePosition(defaultDirection);
            }
            else
            {
                Vector2 target = Vector2.MoveTowards(transform.position, player.transform.position, Time.fixedDeltaTime * speed * GameManager.Instance.generalSpeed);
                rb.MovePosition(target);
            }
        }
        else
        {
            if (pc.isUnderAttack)
            {
                transform.rotation = Quaternion.Euler(0, 0, -58);
                transform.localPosition = new Vector3(0f, 0f, 0);
            }
        }

        if (transform.position.x < -12)
        {
            pc.viewEnemy = null;
            pc.isUnderAttack = false;
            isHitPlayer = false;
            ObjectPool.Instance.DeleteObject(this.gameObject);
        }

        if (hp <= 0)
        {
            transform.parent = null;
            transform.GetComponent<Collider2D>().enabled = false;
            pc.viewEnemy = null;
            pc.isUnderAttack = false;
            isHitPlayer = false;
            isSetDamage = false;
            anim.SetBool("isDead", true);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.MovePosition(defaultDirection);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && isHitPlayer == false)
        {
            isHitPlayer = true;
            pc.isUnderAttack = true;
            anim.SetBool("isAttack", true);
            transform.parent = playerLeg;
            if (!isSetDamage)
            {
                StartCoroutine(SetDamage());
            }
        }
    }

    private void OnDisable()
    {
        hp = 20;
    }

    IEnumerator SetDamage()
    {
        isSetDamage = true;
        while (isHitPlayer)
        {
            yield return new WaitForSeconds(0.5f);
            if (pc.hp >= 10)
            {
                pc.hp -= 10;
            }
        }
    }
}
