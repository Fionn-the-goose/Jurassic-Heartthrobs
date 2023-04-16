using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetNextGoal : MonoBehaviour
{
    public Transform NextPosition;
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.transform.position, 5f);
        Gizmos.DrawSphere(NextPosition.position, 2f);
    }

    private void OnTriggerEnter(Collider other) {
        var dino = other.GetComponent<Dino>();
        Debug.Log("Dino Entert");
        Debug.Log(other);
        if(dino != null){
            var dest = new Vector3(NextPosition.position.x + Random.Range(0f, 3f), NextPosition.position.y ,NextPosition.position.z + Random.Range(0f, 3f));
            dino.SetDestination(dest);
        }
    }
}
