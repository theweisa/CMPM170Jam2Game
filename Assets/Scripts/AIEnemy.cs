using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Kino;
using UnityEngine.UI;

public class AIEnemy : MonoBehaviour
{
    public Canvas CV;
    public Canvas WinCV;

    // stats
    private float baseChaseSpeed = 8f;
    private float baseSearchSpeed = 5f;
    private float detectDist = 8f;
    private float glitchDist = 13f;
    private float peekabooDist = 20f;
    public int chaseLevel = 0;
    private float dist;

    private Vector3 dir;
    private bool gameOver = false;
    
    // timers
    private bool chasing = false;
    private float chaseDuration = 10f;
    private float chaseTimer = 0f;

    private bool chaseCooldown = false;
    private float chaseCooldownDuration = 5f;
    private float chaseCooldownTimer = 0f;
    private float minCuteInterval = 2f;
    private float maxCuteInterval = 5f;
    private float cuteInterval = 5f;
    private float cuteTimer = 0f;

    private float peekabooInterval = 5f;
    private float peekabooTimer;
    private float peekabooPitch = 2f;
    
    private float scaryDuration = 0.3f;
    private float scaryTimer = 0f;
    private bool scary = false;

    private float gameOverTimer = 2.5f;

    private int startScary = 2;
    private int permaScary = 4;


    // obj vars
    public Transform player;
    public AnalogGlitch glitch;
    public Collider rigidCollider;
    private Rigidbody rb;
    private GameObject cuteEnemy;
    private GameObject scaryEnemy;

    public AudioSource peekabooSfx;
    public AudioSource glitchSfx;

    public AudioSource bgm;


    void Start() {
        rb = GetComponent<Rigidbody>();
        cuteEnemy = transform.Find("CuteEnemy").gameObject;
        scaryEnemy = transform.Find("ScaryEnemy").gameObject;
        peekabooTimer = peekabooInterval;
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
            gameOverTimer-=Time.deltaTime;
            if (gameOverTimer <= 0f) {
                GameOver();
            }
            return;
        }
        
        //Check for sight?
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        dist = Vector3.Distance(transform.position, player.position);
        dir = (player.position - transform.position).normalized;
        // always rotate the enemy towards the general direction of the player
        Vector3 pos = ProjectPointOnPlane(transform.up, transform.position, player.position);
        transform.LookAt(pos, transform.up);

        if (scary) {
            cuteEnemy.SetActive(false);
            scaryEnemy.SetActive(true);
            if (chaseLevel <= permaScary) {
                UpdateScary();
            }
        }
        else {
            scaryEnemy.SetActive(false);
            cuteEnemy.SetActive(true);
            if (chaseLevel >= startScary) {
                UpdateCute();
            }
        }

        // player.Rotate(Vector3.up*inputX); 
        // Debug.Log(dist);
        if (dist <= peekabooDist) {
            peekabooTimer += Time.deltaTime;
            if (peekabooTimer >= peekabooInterval) {
                if (!peekabooSfx.isPlaying) peekabooSfx.Play();
                peekabooSfx.pitch = peekabooPitch*(1f-(0.1f*chaseLevel));
                peekabooSfx.volume = ((peekabooDist-dist)/dist)+0.3f;
                peekabooTimer = 0f;
            }
        }
        // apply glitch effect based on distance
        if (dist <= glitchDist) {
            GlitchEffect();
        }
        else {
            SetGlitch(0f);
        }
        if (dist <= detectDist*(1+chaseLevel*0.2) && !chaseCooldown) {
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
    private Vector3 ProjectPointOnPlane(
        Vector3 planeNormal, Vector3 planePoint, Vector3 point
    ) {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(
            planeNormal.normalized, (point - planePoint)
        );
        return (point + planeNormal * distance);
    }

    private void GlitchEffect() {
        float glitchRatio = ((glitchDist-dist)/glitchDist)*0.6f;
        SetGlitch(glitchRatio);
    }

    private void SetGlitch(float glitchVal) {
        glitch.scanLineJitter = glitchVal;
        glitch.verticalJump = glitchVal;
        glitch.horizontalShake = glitchVal;
        glitch.colorDrift = glitchVal;
        if (!glitchSfx.isPlaying) glitchSfx.Play();
        glitchSfx.volume = glitchVal+0.2f;
        if (glitchVal == 0) glitchSfx.Stop();
    }

    private void ChasePlayer() {
        print("chasing player");
        if (!chasing)
            chasing = true;
        else {
            float chaseSpeed = baseChaseSpeed*(1f+(chaseLevel*0.1f));
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
        float searchSpeed = baseSearchSpeed*(1f+(chaseLevel*0.1f));
        rb.velocity = transform.forward*searchSpeed;
    }

    private void UpdateChaseCooldown() {
        chaseCooldownTimer += Time.deltaTime;
        if (chaseCooldownTimer >= chaseCooldownDuration*(1-(chaseLevel*0.1f))) {
            print("can chase again");
            chaseCooldown = false;
            chaseCooldownTimer = 0f;
        }
    }

    private void UpdateCute() {
        cuteTimer += Time.deltaTime;
        if (cuteTimer >= cuteInterval) {
            // change form for a bit
            scary = true;
            cuteTimer = 0f;
            ChangeForm(scary);
            cuteInterval = Random.Range(minCuteInterval*(1f-(chaseLevel*0.15f)), maxCuteInterval*(1f-(chaseLevel*0.15f)));
        }
    }

    private void UpdateScary() {
        scaryTimer += Time.deltaTime;
        if (scaryTimer >= scaryDuration*(1+chaseLevel*0.25f)) {
            scary = false;
            scaryTimer = 0f;
            ChangeForm(scary);
        }
    }

    private void ChangeForm(bool isScary) {
        if (isScary) {
            cuteEnemy.SetActive(false);
            scaryEnemy.SetActive(true);
        }
        else {
            scaryEnemy.SetActive(false);
            cuteEnemy.SetActive(true);
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

    // GAME OVER HERE
    private void GameOver() {
        bgm.mute = true;
        CV.gameObject.SetActive(true);
    }

    public void GameWin()
    {
        WinCV.gameObject.SetActive(true);
    }

    private void JumpScare() {
        player.gameObject.GetComponent<PlayerController>().SetGameOver(true);
        cuteEnemy.SetActive(false);
        scaryEnemy.SetActive(true);
        gameOver = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rigidCollider.enabled = false;
        transform.position = player.position + player.forward*2.5f;
        transform.position = transform.position + transform.up*0.2f;
        transform.eulerAngles = player.eulerAngles;
        transform.Rotate(0f, 180f, 0f);
        // transform.LookAt(player.position);
    }
}
