using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    public event Action<bool> OnFreezeChange = null;

    private bool m_IsFrozen = false;

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

    public void SetFrozen(bool freeze) {
        OnFreezeChange?.Invoke(freeze);
    }
}
