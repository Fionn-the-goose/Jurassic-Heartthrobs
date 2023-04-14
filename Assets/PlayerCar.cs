using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCar : MonoBehaviour {

    [SerializeField]
    private InputActionReference m_MoveAction;

    private Rigidbody m_RigidBody;
    private Camera m_Camera;

    void Start() {
        m_Camera = FindObjectOfType<Camera>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Update() {
        var input = m_MoveAction.action.ReadValue<Vector2>();
        var delta = input * m_Camera.transform.forward;
        Debug.Log(input);
        m_RigidBody.AddForce(delta, ForceMode.Acceleration);
    }
}
