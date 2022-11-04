using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Kino;

public class AIEnemy : MonoBehaviour
{
    private float baseChaseSpeed = 8f;
    private float baseSearchSpeed = 5f;
    private float detectDist = 8f;
    private float glitchDist = 13f;
    private int chaseLevel = 0;

    private bool chasing = false;
    private float chaseDuration = 10f;
    private float chaseTimer = 0f;

    private bool chaseCooldown = false;
    private float chaseCooldownDuration = 5f;
    private float chaseCooldownTimer = 0f;

    private Vector3 dir;
    private bool gameOver = false;

    public Transform player;
    public AnalogGlitch glitch;
    public Collider rigidCollider;
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
    private void FixedUpdate()
    {
        if (gameOver) {
            JumpScare();
            return;
        }
        //Check for sight?
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        float distance = Vector3.Distance(transform.position, player.position);
        dir = (player.position - transform.position).normalized;
        // always rotate the enemy towards the general direction of the player

        Debug.DrawLine(transform.position, transform.position + dir * 10, Color.red, Mathf.Infinity);    

        Vector3 pos = ProjectPointOnPlane(transform.up, transform.position, player.position);
        transform.LookAt(pos, transform.up);

        // player.Rotate(Vector3.up*inputX); 
        Debug.Log(distance);
        // apply glitch effect based on distance
        if (distance <= glitchDist) {
            GlitchEffect(distance);
        }
        else {
            SetGlitch(0f);
        }
        if (distance <= detectDist && !chaseCooldown) {
            ChasePlayer();
        }
        else {
            SearchPlayer();
        }
        if (chaseCooldown) {
            UpdateChaseCooldown();
        }
    }

    // from https://answers.unity.com/questions/648286/character-lookat-on-spherical-planet-face-y-to-tar.html
    private Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
        return (point + planeNormal * distance);
    }

    private void GlitchEffect(float dist) {
        float glitchRatio = ((glitchDist-dist)/glitchDist)*0.6f;
        SetGlitch(glitchRatio);
    }

    private void SetGlitch(float glitchVal) {
        glitch.scanLineJitter = glitchVal;
        glitch.verticalJump = glitchVal;
        glitch.horizontalShake = glitchVal;
        glitch.colorDrift = glitchVal;
    }

    private void ChasePlayer() {
        print("chasing player");
        if (!chasing)
            chasing = true;
        else {
            float chaseSpeed = baseChaseSpeed*(1f+(chaseLevel*0.075f));
            rb.velocity = transform.forward*chaseSpeed;
            /*transform.position = Vector3.MoveTowards(
                transform.position, player.position, Time.deltaTime * chaseSpeed
            );*/
            
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseDuration) {
                print("done chasing");
                chaseCooldown = true;
                chasing = false;
                chaseTimer = 0f;
                chaseCooldown = true;
            }
        }
    }

    private void SearchPlayer() {
        print("searching for player");
        float searchSpeed = baseSearchSpeed*(1f+(chaseLevel*0.075f));
        rb.velocity = transform.forward*searchSpeed;
    }

    private void UpdateChaseCooldown() {
        chaseCooldownTimer += Time.deltaTime;
        if (chaseCooldownTimer >= chaseCooldownDuration*(1f+(chaseLevel*0.75f))) {
            print("can chase again");
            chaseCooldown = false;
            chaseCooldownTimer = 0f;
        }
    }

    public void IncreaseChaseLevel() {
        chaseLevel++;
    }

    private void OnTriggerEnter(Collider obj) {
        if (obj.tag == "Player") {
            JumpScare();
        }
    }

    private void JumpScare() {
        player.gameObject.GetComponent<PlayerController>().SetGameOver(true);
        gameOver = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rigidCollider.enabled = false;
        transform.position = player.position + player.forward*2f;
        transform.rotation = player.rotation;
        transform.LookAt(player.position);
    }
}
