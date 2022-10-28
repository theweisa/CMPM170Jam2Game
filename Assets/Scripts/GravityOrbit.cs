using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{
    // center that objects orbit
    public float gravity;

    private void OnTriggerEnter(collider obj) {
        if (obj.GetComponent<GravityControl>()) {
            obj.GetComponent<GravityControl>().gravityObj = this.GetComponent<GravityOrbit>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
