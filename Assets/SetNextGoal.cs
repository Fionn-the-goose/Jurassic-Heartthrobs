using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetNextGoal : MonoBehaviour
{
    public Transform NextPosition;
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.transform.position, 6f);
        Gizmos.DrawSphere(NextPosition.position, 2f);
    }

    private void OnTriggerEnter(Collider other) {
        var dino = GetComponent<Dino>();
        if(dino != null){
            dino.DinoAgend.destination = transform.position;
        }
    }
}
