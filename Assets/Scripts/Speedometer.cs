using System;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour {
    private TMP_Text m_Text;
    private PlayerCar m_Player;

    void Start() {
        m_Text = GetComponent<TMP_Text>();
        m_Player = FindObjectOfType<PlayerCar>();
    }

    void Update() {
        var speed = Math.Round(13f * m_Player.Velocity, 2);
        m_Text.text = speed + " km/h";
    }
}
