using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<PlayerCar>();
        if(player != null){
            player.Boost(5);
        }
    }
}
