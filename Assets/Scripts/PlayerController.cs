using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private Rigidbody rb;

    public float speed = 10f;
    public float turnSmoothTime = 0.1f; 
    // public float jumpHeight = 1.0f;
    // public float gravityValue = -9.81f;

    private float turnSmoothVelocity;

    private Vector3 velocity;
    private bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        // get axis' of movement (?)
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (horiz <= 0.1f && vert <= 0.1f) {
            rb.velocity = Vector3.zero;   
        }

        Vector3 dir = new Vector3(horiz, rb.velocity.y, vert).normalized;
        // dir = cam.transform.TransformDirection(dir);

        rb.velocity = dir*speed;
    }
}
