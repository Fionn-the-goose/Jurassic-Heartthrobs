using UnityEngine;
using Yarn.Unity;

public class DialogeTrigger : MonoBehaviour {

    public DialogueRunner dialogueRunner;
    private bool m_DialogueRunning;
    private float m_LastInteracton = 0f;
    public const float COOLDOWN = 5f;

    private void OnTriggerEnter(Collider other) {
        var date = other.GetComponent<Dino>();
        if (date != null && !dialogueRunner.IsDialogueRunning
                && m_LastInteracton + COOLDOWN < Time.fixedTime) {
            m_LastInteracton = Time.fixedTime;
            m_DialogueRunning = true;
            GameManager.Instance.SetFrozen(true);
            dialogueRunner.StartDialogue(date.name + "Start");
        }
    }

    private void Update() {
        // dialogue just stopped
        if (m_DialogueRunning && !dialogueRunner.IsDialogueRunning) {
            m_LastInteracton = Time.fixedTime;
            GameManager.Instance.SetFrozen(false);
        }
        m_DialogueRunning = dialogueRunner.IsDialogueRunning;
    }
}
