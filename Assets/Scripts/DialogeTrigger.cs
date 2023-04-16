using UnityEngine;
using Yarn.Unity;

public class DialogeTrigger : MonoBehaviour {

    public DialogueRunner dialogueRunner;
    private bool m_DialogueRunning;
    private float m_LastInteracton = 0f;
    public const float COOLDOWN = 5f;
    private PlayerCar m_Car;

    void Start() {
        if (dialogueRunner == null) {
            dialogueRunner = FindObjectOfType<DialogueRunner>();
        }
        m_Car = GetComponent<PlayerCar>();
    }

    private void OnTriggerEnter(Collider other) {
        var date = other.GetComponent<Dino>();
        Debug.Log($"dashing? {m_Car.IsDashing()}");
        if (date != null && !dialogueRunner.IsDialogueRunning) {
            if (m_Car.IsDashing() && m_LastInteracton + COOLDOWN < Time.fixedTime) {
                TriggerDialogue(date.name);
            } else {
                GameManager.PlaySFX("SoundCrash");
            }
        }
    }

    private void TriggerDialogue(string date_name) {
        GameManager.SetupUI(date_name);
        m_LastInteracton = Time.fixedTime;
        m_DialogueRunning = true;
        m_Car = GetComponent<PlayerCar>();
        GameManager.Instance.SetFrozen(true);
        dialogueRunner.StartDialogue(date_name + "Start");
        GameManager.ChangeMusic("Music" + date_name);
        GameManager.PlaySFX("SoundSmash");
    }

    private void Update() {
        // dialogue just stopped
        if (m_DialogueRunning && !dialogueRunner.IsDialogueRunning) {
            m_LastInteracton = Time.fixedTime;
            GameManager.Instance.SetFrozen(false);
        }
        m_DialogueRunning = dialogueRunner.IsDialogueRunning;
        if (Input.GetKeyDown(KeyCode.N)) {
            TriggerDialogue("Manu");
        }
    }
}
