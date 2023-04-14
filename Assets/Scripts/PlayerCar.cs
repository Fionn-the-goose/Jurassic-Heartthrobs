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

    public bool GoingForward {
        get {
            // forward = 0. reverse = 180
            var theta = Vector3.SignedAngle(transform.forward, m_RigidBody.velocity, Vector3.up);
            return theta > -90f && theta < 90f;
        }
    }

    void Start() {
        m_Camera = FindObjectOfType<Camera>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        var delta_t = Time.fixedDeltaTime;
        var input = m_MoveAction.action.ReadValue<Vector2>();
        var delta_pos = delta_t * m_Speed * transform.forward * input.y;
        var fwd = GoingForward ? 1 : -1;
        m_RigidBody.AddForce(delta_pos, ForceMode.Acceleration);
        m_RotAngle += fwd * m_TurnSens * input.x * delta_t * (Velocity/3.5f);
        var rot = Quaternion.AngleAxis(m_RotAngle, transform.up);
        m_RigidBody.MoveRotation(rot);

    }
}
