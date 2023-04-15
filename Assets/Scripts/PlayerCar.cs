using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCar : MonoBehaviour {

    [SerializeField]
    private InputActionReference
        m_MoveAction,
        m_DashLeftAction,
        m_DashRightAction;

    [SerializeField]
    private float m_TurnSens;

    [SerializeField]
    private float m_Speed;

    private float m_MaxSpeed = 6f;

    [SerializeField]
    private float m_DashForce;

    [SerializeField]
    private bool m_BounceOutOfBounds = true;

    private Rigidbody m_RigidBody;
    private Camera m_Camera;
    private float m_RotAngle = 0f;

    public float Velocity {
        get => m_RigidBody.velocity.magnitude;
    }

    public float SignedVelocity {
        get => Velocity * (GoingForward ? 1f : -1f);
    }

    public float SteeringInput {
        get => m_MoveAction.action.ReadValue<Vector2>().x;
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
        UpdateMovement(delta_t);
        UpdateDash(delta_t);
    }

    void UpdateMovement(float delta_t) {
        var input = m_MoveAction.action.ReadValue<Vector2>();
        var fwd = GoingForward ? 1 : -1;

        m_RotAngle += fwd * m_TurnSens * input.x * delta_t * (Velocity/4.5f);
        var rot = Quaternion.AngleAxis(m_RotAngle, transform.up);
        m_RigidBody.MoveRotation(rot);

        var accel = delta_t * m_Speed * transform.forward * input.y;
        if (Velocity < m_MaxSpeed || Input.GetKey(KeyCode.P)) {
            m_RigidBody.AddForce(accel, ForceMode.Acceleration);
        }
    }

    void UpdateDash(float delta_t) {
        float direction = 0f;
        if (m_DashLeftAction.action.WasPressedThisFrame()) {
            direction = -1f;
        } else if (m_DashRightAction.action.WasPressedThisFrame()) {
            direction = 1f;
        } else {
            return;
        }
        m_RigidBody.AddForce(m_DashForce * direction * (transform.right + 0.3f * transform.forward), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Track")) {
            return;
        }
        foreach (ContactPoint contact in collision.contacts) {
            float f = m_BounceOutOfBounds ? 1f : 0.4f;
            var force = 5f * f * collision.relativeVelocity.magnitude * contact.normal / collision.contacts.Length;
            m_RigidBody.AddForce(force, ForceMode.Impulse);
            Debug.Log("hit wall");
        }
    }

}
