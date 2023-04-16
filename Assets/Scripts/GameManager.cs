using Yarn.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    public event Action<bool> OnFreezeChange = null;

    private bool m_IsFrozen = false;

    [SerializeField]
    private List<AudioClip> m_Music = new List<AudioClip>();

    private AudioSource m_MusicSource;

    private List<string> m_Dates = new List<string>();

    public string CurrentDino = "";

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
        Instance.m_Dates.Add(dinoName);
        Debug.Log($"Dating {dinoName}!");
        foreach (var p in FindObjectsOfType<DatingPlatform>()) {
            if (p.DinoName == dinoName) {
                p.SetVisible(true);
            }
        }
        foreach (var d in FindObjectsOfType<Dino>()) {
            if (d.Name == dinoName) {
                Destroy(d.gameObject);
            }
        }
        SetupUI(dinoName, hide: true, smash: true);
    }

    [YarnCommand("date_fail")]
    public static void DateFail(string dinoName) {
        SetupUI(dinoName, hide: true, smash: false);
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
            Instance.m_MusicSource.loop = true;
        }
    }

    public static void SetupUI(string dinoname, bool hide = false, bool smash = true) {
        var ui = GameObject.Find("UI Animator").GetComponent<Animator>();
        foreach (var im in ui.GetComponentsInChildren<Image>(true)) {
            if (im.name == $"{dinoname}Icon") {
                im.enabled = true;
            } else if (im.name == "PassIcon") {
                im.enabled = hide && !smash;
                Debug.Log("Pass:  " + im);
            } else if (im.name == "SmashIcon") {
                im.enabled = hide && smash;
                Debug.Log("Smash:  " + im);
            } else if (im.name.EndsWith("Icon") && im.name != "TextIcon") {
                im.enabled = false;
            }
        }
        if (hide) {
            ui.SetTrigger(smash ? "Smash" : "Pass");
            Instance.CurrentDino = "";
        } else {
            ui.SetTrigger("FadeIn");
            Instance.CurrentDino = dinoname;
        }
    }

    public bool IsDating(string dinoname) {
        Debug.Log(Instance.m_Dates);
        return Instance.m_Dates.Contains(dinoname);
    }

}
