using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum PlayerState
{
    walk,
    attack,
    death
}

public class PlayerController : MonoBehaviour
{
    public float speed;
    public PlayerState currentState;
    [SerializeField] private float hp;

    private Rigidbody2D rb;
    private new SpriteRenderer renderer;
    private Animator animator;
    private Vector3 movement;
    private bool walking;
    private bool lockDeath;
    private bool lockDamage;

    void Start()
    {
        currentState = PlayerState.walk;
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (currentState != PlayerState.death)
        {
            movement = Vector3.zero;
            movement.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
            movement.y = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
            //movement = new Vector3(movement.x, movement.y).normalized;

            if (currentState == PlayerState.walk)
            {
                UpdateAnimationAndMove();
            }

            if (Input.GetMouseButtonDown(0) && currentState != PlayerState.attack)
            {
                StartCoroutine(AttackCo());
            }

            if (hp <= 0)
            {
                if (!lockDeath)
                {
                    lockDeath = true;
                    animator.SetTrigger("Death");
                    Death();
                }
            }
        }
    }

    public void Death()
    {
        currentState = PlayerState.death;
        //back to menu or restart
    }

    private void UpdateAnimationAndMove()
    {
        if (movement != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            StopMoving();
        }
    }
    
    private void MoveCharacter()
    {
        transform.Translate(new Vector3(movement.x, movement.y));
    }

    private void StopMoving()
    {
        rb.velocity = Vector3.zero;
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("isAttack", true);
        currentState = PlayerState.attack;
        yield return null;
        animator.SetBool("isAttack", false);
        yield return new WaitForSeconds(.15f);
        currentState = PlayerState.walk;
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
        yield return new WaitForSeconds(.5f);
        lockDamage = false;
    }
}
