using UnityEngine;
using UnityEngine.AI;
public class Dino : MonoBehaviour {
    public string Name;
    public Transform InitialGoal;
    public NavMeshAgent DinoAgend{get; set;}
    private Vector3 m_Dest;  // keeping in case SetDestination is called while dino asleep
    private void Start() {
        DinoAgend = GetComponent<NavMeshAgent>();
        DinoAgend.destination = InitialGoal.position;
        GameManager.Instance.OnFreezeChange += (bool frozen) => {
            DinoAgend.enabled = !frozen;
            DinoAgend.GetComponent<Rigidbody>().isKinematic = frozen;
            if (!frozen) {
                DinoAgend.destination = m_Dest;
            }
        };
    }
    public void SetDestination(Vector3 dest) {
        if (DinoAgend.enabled) {
            DinoAgend.destination = dest;
        }
        m_Dest = dest;
    }
    private void Update() {
    }
}
