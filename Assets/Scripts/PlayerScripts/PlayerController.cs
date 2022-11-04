using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public Animator walkAnim;
    private Rigidbody rb;
    public float speed;
    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (gameOver) return;
        // get axis' of movement (?)
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(horiz, 0f, vert).normalized;
        dir = transform.TransformDirection(dir);
        if (Mathf.Abs(horiz) <= 0.1f && Mathf.Abs(vert) <= 0.1f) {
            walkAnim.enabled = false;
            rb.velocity = Vector3.zero;
        }
        else {
            walkAnim.enabled = true;
            rb.velocity = dir*speed;
        }
    }

    public void SetGameOver(bool state) {
        gameOver = state;
    }

    public bool GetGameOver() {
        return gameOver;
    }
}
