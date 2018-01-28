using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEvent : MonoBehaviour {

    public GameObject droneAnimator;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8) //player
        {
            droneAnimator.GetComponent<Animator>().SetTrigger("StartFlag");
        }
    }
}
