using UnityEngine;
using UnityEngine.AI;
public class Dino : MonoBehaviour {
    public string Name;
    public Transform InitialGoal;
    public NavMeshAgent DinoAgend{get; set;}
    private void Start() {
        DinoAgend = GetComponent<NavMeshAgent>();
        DinoAgend.destination = InitialGoal.position;
    }
    private void Update() {
    }
}