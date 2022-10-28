using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    // lets object rotate around GravityOrbit obj
    public GravityOrbit gravityObj;
    private Rigidbody rb;

    public float rotationSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // if there is a planet to orbit
        if (gravityObj) {
            Vector3 gravityUp = Vector3.zero;
            // get the obj pos compared to the gravity obj pos
            gravityUp = (transform.position-gravityObj.transform.position).normalized;

            Vector3 localUp = transform.up;

            Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp)*transform.rotation;
            
            transform.up = Vector3.Lerp(transform.up, gravityUp, rotationSpeed*Time.deltaTime);

            rb.AddForce((-gravityUp*gravityObj.gravity)*rb.mass);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
