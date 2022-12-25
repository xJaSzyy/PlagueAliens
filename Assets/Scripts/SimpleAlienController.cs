using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlienController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float hp;
    [SerializeField] private float damage;
    [SerializeField] private float maxRange;
    [SerializeField] private float minRange;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform target;
    private new SpriteRenderer renderer;
    private bool lockDamage;
    private bool lockDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        target = FindObjectOfType<PlayerController>().transform;
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) <= maxRange && Vector3.Distance(target.position, transform.position) >= minRange && hp > 0)
        {
            FollowPlayer();
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (hp <= 0)
        {
            if (!lockDeath)
            {
                lockDeath = true;
                animator.SetTrigger("Death");
            }
            
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    private void FollowPlayer()
    {
        animator.SetBool("isMoving", true);
        animator.SetFloat("Horizontal", (target.position.x - transform.position.x));
        animator.SetFloat("Vertical", (target.position.y - transform.position.y));
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        if (!lockDamage)
        {
            lockDamage = true;
            StartCoroutine(Hit(damage));
        }
    }

    IEnumerator Hit(float damage)
    {
        renderer.color = Color.red;
        hp -= damage;
        yield return new WaitForSeconds(.1f);
        renderer.color = Color.white;
        lockDamage = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
