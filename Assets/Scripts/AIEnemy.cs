using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{

    public Transform player;
    
    // Update is called once per frame
    private void Update()
    {
        //Check for sight?
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        var distance = Vector3.Distance(transform.position, player.position);
        Debug.Log(distance);
        if(distance <= 6f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * 2);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * distance * 2);
        }


        //sees player
        //if (playerInSightRange) ChasePlayer();
        //attack/jumpscare?
        //if (playerIn)
    }
}
