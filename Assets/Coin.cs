using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<PlayerCar>();
        if(player != null){
            player.DinoCoins ++;
            player.SetDialougeCoins();
            Destroy(gameObject);
        }
    }
}
