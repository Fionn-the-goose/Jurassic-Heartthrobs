using UnityEngine;
using Yarn.Unity;

public class TriggerDialog : MonoBehaviour {
    
    public DialogueRunner dialogueRunner;

    private void OnTriggerEnter(Collider other) {
        var date = other.GetComponent<Dino>();
        if(date != null){
            dialogueRunner.StartDialogue(date.name);
        }
    }
}