using UnityEngine;
using UnityEngine.AI;
public class Dino : MonoBehaviour {
    public string Name;
    public NavMeshAgent DinoAgend{get; set;}
    private void Start() {
        DinoAgend = GetComponent<NavMeshAgent>();
    }
    private void Update() {
        
    }
}