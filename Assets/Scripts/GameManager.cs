using Yarn.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    public event Action<bool> OnFreezeChange = null;

    private bool m_IsFrozen = false;

    [SerializeField]
    private List<AudioClip> m_Music = new List<AudioClip>();

    private AudioSource m_MusicSource;

    public bool Frozen {
        get => m_IsFrozen;
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    void Start() {
        m_MusicSource = GameObject.Find("CM vcam1").GetComponent<AudioSource>();
    }

    public void SetFrozen(bool freeze) {
        OnFreezeChange?.Invoke(freeze);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            DateSuccess("Manu");
        }
    }

    [YarnCommand("date_success")]
    public static void DateSuccess(string dinoName) {
        foreach (var p in FindObjectsOfType<DatingPlatform>()) {
            if (p.DinoName == dinoName) {
                p.SetVisible(true);
                Debug.Log($"Datin {dinoName}!");
            }
        }
        foreach (var d in FindObjectsOfType<Dino>()) {
            if (d.Name == dinoName) {
                Destroy(d.gameObject);
            }
        }
    }

    [YarnCommand("change_music")]
    public static void ChangeMusic(string name) {
        AudioClip clip = null;
        foreach (var c in Instance.m_Music) {
            if (c.name == name) {
                clip = c;
                break;
            }
        }
        if (clip != null) {
            Instance.m_MusicSource.Stop();
            Instance.m_MusicSource.PlayOneShot(clip);
        }
    }
}
