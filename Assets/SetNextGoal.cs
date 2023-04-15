using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetNextGoal : MonoBehaviour
{
    public Transform NextPosition;
    private void OnTriggerEnter(Collider other) {
        var dino = GetComponent<Dino>();
        if(dino != null){
            dino.DinoAgend.destination = transform.position;
        }
    }
}
