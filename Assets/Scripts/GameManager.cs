using Yarn.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    public event Action<bool> OnFreezeChange = null;

    private bool m_IsFrozen = false;

    [SerializeField]
    private List<AudioClip> m_Music = new List<AudioClip>();

    private AudioSource m_MusicSource;
    private AudioSource m_FXSource;
    private AudioSource m_VoiceSource;

    private TMP_Text m_CharacterName;
    private string m_Speaker = null;

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
        m_FXSource = GameObject.Find("FX Source").GetComponent<AudioSource>();
        m_VoiceSource = GameObject.Find("Voice Source").GetComponent<AudioSource>();
        m_CharacterName = GameObject.Find("Character Name").GetComponent<TMP_Text>();
        ChangeMusic("RacingMusic");
        PlaySFX("SoundSmash");
    }

    public void SetFrozen(bool freeze) {
        OnFreezeChange?.Invoke(freeze);
    }

    void Update() {
        var speaker_new = m_CharacterName.text;

        if (m_Speaker != speaker_new) {
            Debug.Log($"New speaker {speaker_new}.");
            ChangeMusic($"Voice{speaker_new}", is_music: false);
        }

        m_Speaker = speaker_new;
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
        ChangeMusic("RacingMusic");
        Instance.m_VoiceSource.Stop();
        GameObject.Find("Partikel Heart").GetComponentInChildren<ParticleSystem>().Emit(30);
    }

    [YarnCommand("date_fail")]
    public static void DateFail(string dinoName) {
        SetupUI(dinoName, hide: true, smash: false);
        ChangeMusic("RacingMusic");
        PlaySFX("SoundFail");
        Instance.m_VoiceSource.Stop();
    }

    [YarnCommand("change_music")]
    public static void ChangeMusic(string name, bool is_music = true) {
        AudioClip clip = null;
        foreach (var c in Instance.m_Music) {
            if (c.name == name) {
                clip = c;
                break;
            }
        }
        if (clip != null) {
            var source = is_music ? Instance.m_MusicSource : Instance.m_VoiceSource;
            source.Stop();
            source.clip = clip;
            if (is_music) {
                source.volume = name == "RacingMusic" ? 1f : 0.4f;
            }
            source.Play();
            source.loop = true;
        }
    }

    [YarnCommand("play_sfx")]
    public static void PlaySFX(string name) {
        AudioClip clip = null;
        foreach (var c in Instance.m_Music) {
            if (c.name == name) {
                clip = c;
                break;
            }
        }
        if (clip != null) {
            Instance.m_FXSource.PlayOneShot(clip);
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

    [YarnCommand("boost_mark")]
    public static void BoostMark() {
        foreach (var agent in FindObjectsOfType<NavMeshAgent>()) {
            if (agent.name == "Mark") {
                agent.speed = 7.8f;
            }
        }
    }
}
