﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emerald : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public int damage = 1;

    public GameObject impactEffect;
    void Start()
    {
        rb.velocity = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized * speed;
    }


    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        JotaroHealth jotaro = hitInfo.GetComponent<JotaroHealth>();
        if (jotaro != null)
        {
            if (Input.GetKey(KeyCode.P) == false)
            {
                jotaro.TakeDamage(damage);
            }
        }

        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);    
    }
    void OnBecameInvisible()
    {
        enabled = false;
        Destroy(gameObject);
    }
}
