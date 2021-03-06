﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dio : MonoBehaviour
{
    public int maxHealth = 20;
    int currentHealth;

    public Animator dioAnim;
    public GameObject knifePrefab;
    public Transform firePoint;
    Transform player;

    public Rigidbody2D rb;
    public float speed = 2.5f;

    public float attackRange = 1f;
    bool attack = false;
    bool start = false;
    bool up = false;
    bool move = false;
    bool down = false;
    public Transform startPos;
    public Transform upPos;
    public JotaroMovement moveScript;
    public JotaroHealth jotaro;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        currentHealth = maxHealth;
        FindObjectOfType<AudioManager>().Play("DioIntro");
        StartCoroutine("AutoAttack");
    }
    
    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            Vector2 target = new Vector2(startPos.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (Vector2.Distance(startPos.position, rb.position) <= 0.1)
            {
                transform.Rotate(0f, 180f, 0f);
                start = false;
            }
        }
        if (move)
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (Vector2.Distance(player.position, rb.position) <= attackRange)
            {
                if (attack)
                {
                    dioAnim.SetTrigger("Attack");
                    attack = false;
                    GameObject go = GameObject.FindGameObjectWithTag("Player");
                    JotaroCombat jotaroCombat = go.GetComponent<JotaroCombat>();
                    if (Input.GetKey(KeyCode.P) && Time.time >= jotaroCombat.nextBlockTime)
                    {
                        jotaroCombat.Block();
                    }
                    else
                    {
                        jotaro.TakeDamage(2);
                    }
                    attack = false;
                }
                move = false;

            }
        }
        if (up)
        {
            Vector2 target = new Vector2(rb.position.x, upPos.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (Vector2.Distance(upPos.position, rb.position) <= 0.1)
            {
                up = false;
                move = true;
                dioAnim.SetTrigger("isFalling");
                FindObjectOfType<AudioManager>().Play("RoadRoller");
            }
        }
        if (down)
        {
            Vector2 target = new Vector2(rb.position.x, player.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (Vector2.Distance(player.position, rb.position) <= 0.5)
            {
                dioAnim.SetTrigger("RR");
                FindObjectOfType<AudioManager>().Play("FastMuda");
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                JotaroCombat jotaroCombat = go.GetComponent<JotaroCombat>();
                if (Input.GetKey(KeyCode.P) && Time.time >= jotaroCombat.nextBlockTime)
                {
                    jotaroCombat.Block();
                }
                else
                {
                    jotaro.TakeDamage(10);
                }
                down = false;
            }
        }
    }

    IEnumerator AutoAttack()
    {
        yield return new WaitForSeconds(4);
        while (currentHealth > 10)
        {
            yield return new WaitForSeconds(2);
            dioAnim.SetTrigger("Knife");
        }
        while (currentHealth <= 10 && currentHealth > 1)
        {
            if (Random.Range(0, 2) == 1)
            {
                Attack();
                yield return new WaitForSeconds(2);
                backToStart();
                yield return new WaitForSeconds(2);
            }
            else
            {
                dioAnim.SetTrigger("Knife");
                yield return new WaitForSeconds(2);
            }
        }
        if (currentHealth == 1)
        {
            RoadRoller();
            yield return new WaitForSeconds(1.5f);
            up = true;
            yield return new WaitForSeconds(2);
            down = true;
            yield return new WaitForSeconds(5);
            transform.position = startPos.position;
            moveScript.enabled = true;

            while (currentHealth == 1)
            {
                if (Random.Range(0, 2) == 1)
                {
                    Attack();
                    yield return new WaitForSeconds(2);
                    backToStart();
                    yield return new WaitForSeconds(2);
                }
                else
                {
                    dioAnim.SetTrigger("Knife");
                    yield return new WaitForSeconds(2);
                }
            }
        }
    }
    void RoadRoller()
    {
        moveScript.jotaroAnim.SetBool("isRunning", false);
        moveScript.enabled = false;
        moveScript.rb.velocity = transform.right * 0;
        FindObjectOfType<AudioManager>().Play("ZaWarudo");
        FindObjectOfType<AudioManager>().Play("ZW");
        dioAnim.SetTrigger("ZW");
    }
    void backToStart()
    {
        transform.Rotate(0f, 180f, 0f);
        dioAnim.SetTrigger("isRunning");
        start = true;
    }
    public void Attack()
    {
        dioAnim.SetTrigger("isRunning");
        move = true;
        attack = true;
    }
    public void Shoot()
    {
        Instantiate(knifePrefab, firePoint.position, firePoint.rotation);
    }

    public void TakeDamage(int damage)
    {
        dioAnim.SetTrigger("Hurt");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dioAnim.SetBool("isDead", true);
        Invoke("LoadNextScene", 4);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
