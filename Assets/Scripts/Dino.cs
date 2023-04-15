using UnityEngine;
using UnityEngine.AI;
public class Dino : MonoBehaviour {
    public string Name;
    private NavMeshAgent DinoAgend;
    private void Start() {
        DinoAgend = GetComponent<NavMeshAgent>();
    }
    private void Update() {
        
    }
}