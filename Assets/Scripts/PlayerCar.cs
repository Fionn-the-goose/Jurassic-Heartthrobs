using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCar : MonoBehaviour {

    [SerializeField]
    private InputActionReference m_MoveAction;

    [SerializeField]
    private float m_TurnSens;

    [SerializeField]
    private float m_Speed;

    private Rigidbody m_RigidBody;
    private Camera m_Camera;
    private float m_RotAngle = 0f;

    public float Velocity {
        get => m_RigidBody.velocity.magnitude;
    }

    void Start() {
        m_Camera = FindObjectOfType<Camera>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Update() {
        var input = m_MoveAction.action.ReadValue<Vector2>();
        var delta = m_Speed * transform.forward * input.y;
        var torque = m_TurnSens * input.x * transform.up;
        m_RigidBody.AddForce(delta, ForceMode.Acceleration);
        // m_RigidBody.AddTorque(torque, ForceMode.Force);
        m_RotAngle += m_TurnSens * input.x;
        var rot = Quaternion.AngleAxis(m_RotAngle, transform.up);
        m_RigidBody.MoveRotation(rot);
    }
}
