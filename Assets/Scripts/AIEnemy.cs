using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{
    private float baseChaseSpeed = 4f;
    private float baseSearchSpeed = 1.5f;
    private float detectDist = 8f;
    private int chaseLevel = 0;

    private bool chasing = false;
    private float chaseDuration = 5f;
    private float chaseTimer = 0f;

    private bool chaseCooldown = false;
    private float chaseCooldownDuration = 5f;
    private float chaseCooldownTimer = 0f;

    public Transform player;
    private Rigidbody rb;
    
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    /*
        move towards the player slower if not in sight.
        move towards the player faster while in sight, but eventually slows down a bit
        the chase duration and search speed will scale with notes, but not chase speed except slightly
        speed is always capped at the player speed
        collecting a note should stun the enemy for a bit before it begins the chase again
    */
    private void Update()
    {
        //Check for sight?
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        var distance = Vector3.Distance(transform.position, player.position);
        Debug.Log(distance);
        if (distance <= detectDist && !chaseCooldown)
        {
            ChasePlayer();
        }
        else
        {
            SearchPlayer();
        }

        if (chaseCooldown) {
            UpdateChaseCooldown();
        }
        //sees player
        //if (playerInSightRange) ChasePlayer();
        //attack/jumpscare?
        //if (playerIn)
    }

    private void ChasePlayer() {
        if (!chasing)
            chasing = true;
        else {
            float chaseSpeed = baseChaseSpeed*(1f+(chaseLevel*0.075f));
            transform.position = Vector3.MoveTowards(
                transform.position, player.position, Time.deltaTime * chaseSpeed
            );
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseDuration) {
                chaseCooldown = true;
                chasing = false;
                chaseTimer = 0f;
                chaseCooldown = true;
            }
        }
    }

    private void SearchPlayer() {
        float searchSpeed = baseSearchSpeed*(1f+(chaseLevel*0.075f));
        transform.position = Vector3.MoveTowards(
            transform.position, player.position, Time.deltaTime * searchSpeed
        );
    }

    private void UpdateChaseCooldown() {
        chaseCooldownTimer += Time.deltaTime;
        if (chaseCooldownTimer >= chaseCooldownDuration*(1f+(chaseLevel*0.75f))) {
            chaseCooldown = false;
            chaseCooldownTimer = 0f;
        }
    }

    public void IncreaseChaseLevel() {
        chaseLevel++;
    }
}
