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
        /*
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 relativeDir = vert * forward + horiz * right;
        */
        /*if (Mathf.Abs(horiz) <= 0.1f && Mathf.Abs(vert) <= 0.1f) {
            rb.velocity = Vector3.zero;
        }*/

        Vector3 dir = new Vector3(horiz, 0f, vert).normalized;
        dir = transform.TransformDirection(dir);

        //dir = Quaternion.Euler(0,cam.transform.eulerAngles.y,0)*(dir);
        // rb.velocity = relativeDir*speed;
        rb.velocity = dir*speed;
        // transform.Translate(relativeDir*speed*Time.deltaTime);
    }
}
