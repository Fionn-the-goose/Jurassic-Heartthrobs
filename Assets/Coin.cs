using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private AudioSource m_CoinSound;
    private void Start() {
        m_CoinSound = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<PlayerCar>();
        if(player != null){
            player.DinoCoins ++;
            player.SetDialougeCoins();
            m_CoinSound.Play();
            //player.Stonken();
            Destroy(gameObject);
        }
    }
}
