using System.Collections.Generic;
using UnityEngine;

public class DatingPlatform : MonoBehaviour {
    public string DinoName {
        get {
            if (Character != null) {
                return Character.name;
            }
            return "null";
        }
    }

    public Transform Character = null;

    private List<Renderer> m_Renderers;

    void Awake() {
        m_Renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        if (Character == null) {
            Debug.LogWarning($"Player kart:  No character found for platform {name}.");
        }
        m_Renderers.AddRange(Character.GetComponentsInChildren<Renderer>());
        SetVisible(false);
    }

    public void SetVisible(bool visible) {
        foreach (var rend in m_Renderers) {
            rend.enabled = visible;
        }
    }
}
