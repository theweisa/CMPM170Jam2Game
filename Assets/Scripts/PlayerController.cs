using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private Rigidbody rb;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // get axis' of movement (?)
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (Mathf.Abs(horiz) <= 0.1f && Mathf.Abs(vert) <= 0.1f) {
            rb.velocity = Vector3.zero;
        }

        Vector3 dir = new Vector3(horiz, 0f, vert).normalized;
        // dir = cam.transform.TransformDirection(dir);

        dir = Quaternion.Euler(0,cam.transform.eulerAngles.y,0)*(dir*speed);

        rb.velocity = dir;
        // rb.AddForce(dir*speed);
    }
}
