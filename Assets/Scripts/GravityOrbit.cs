using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{
    // center that objects orbit
    public float gravity;

    // Start is called before the first frame update
    void Start()
    {
        print("poggers");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider obj) {
        if (obj.GetComponent<GravityControl>()) {
            obj.GetComponent<GravityControl>().gravityObj = this.GetComponent<GravityOrbit>();
        }
    }       
}
