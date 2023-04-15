using UnityEngine;
using Yarn.Unity;

public class DialogeTrigger : MonoBehaviour {
    
    public DialogueRunner dialogueRunner;

    private void OnTriggerEnter(Collider other) {
        var date = other.GetComponent<Dino>();
        if(date != null){
            Debug.Log(date.name + "Start");
/*             Time.timeScale = 0f;*/
            dialogueRunner.StartDialogue(date.name + "Start");
        }
    }
}