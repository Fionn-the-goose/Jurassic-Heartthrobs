using System.Collections.Generic;
using UnityEngine;

public class DatingPlatform : MonoBehaviour {
    public string DinoName;

    private List<Renderer> m_Renderers;

    void Awake() {
        m_Renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        SetVisible(false);
    }

    public void SetVisible(bool visible) {
        foreach (var rend in m_Renderers) {
            rend.enabled = visible;
        }
    }
}
