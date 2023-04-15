using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    [SerializeField]
    private Transform m_Kart;

    private float m_MaxSpeed = 10f;

    [SerializeField]
    private float m_DashForce;

    [SerializeField]
    private bool m_BounceOutOfBounds = true;

    private Rigidbody m_RigidBody;
    private Camera m_Camera;
    private float m_RotAngle = 0f;

    private bool m_Sleeping = false;
    private Vector3 m_PreSleepVel;
    private Vector3 m_PreSleepAngVel;

    private float m_DriftOffset = 0f;
    private Vector3 m_DashOffsetTarget = Vector3.zero;
    private Vector3 m_KartInitLocalPos;

    private Coroutine m_DashCoroutine = null;

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
        m_KartInitLocalPos = m_Kart.transform.localPosition;
        GameManager.Instance.OnFreezeChange += (bool freeze) => {
            SetFrozen(freeze);
        };
    }

    void SetFrozen(bool freeze) {
        if (freeze) {
            m_PreSleepVel = m_RigidBody.velocity;
            m_PreSleepAngVel = m_RigidBody.angularVelocity;
            m_RigidBody.isKinematic = true;
        } else {
            m_RigidBody.isKinematic = false;
            m_RigidBody.AddForce(m_PreSleepVel, ForceMode.VelocityChange);
            m_RigidBody.AddTorque(m_PreSleepAngVel, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate() {
        if (GameManager.Instance.Frozen) {
            return;
        }
        var delta_t = Time.fixedDeltaTime;

        UpdateDrift(delta_t);
        UpdateDash(delta_t);

        UpdateMovement(delta_t);
    }

    void UpdateMovement(float delta_t) {
        var input = m_MoveAction.action.ReadValue<Vector2>();
        var fwd = GoingForward ? 1 : -1;

        float f = Velocity < 0.1f ? 0f : 1f;
        m_RotAngle += fwd * m_TurnSens * input.x * delta_t * Mathf.Max(f, (Velocity/4.5f));
        var rot = Quaternion.AngleAxis(m_RotAngle + m_DriftOffset, transform.up);
        m_RigidBody.MoveRotation(rot);

        var accel = delta_t * m_Speed * transform.forward * input.y;
        if (Velocity < m_MaxSpeed || Input.GetKey(KeyCode.P)) {
            m_RigidBody.AddForce(accel, ForceMode.Acceleration);
        }
    }

    void UpdateDrift(float delta_t) {
        var input = m_MoveAction.action.ReadValue<Vector2>().x;
        float target = 0f;
        if (Input.GetKey(KeyCode.Space) && Mathf.Abs(input) > 0.5f) {
            target = Mathf.Sign(input) * 35f;
            if (Input.GetKeyDown(KeyCode.Space)) {
                m_RigidBody.AddForce(40f * Vector3.up, ForceMode.Impulse);
            }
        }
        var delta = target - m_DriftOffset;
        m_DriftOffset += delta * delta_t * 4f;
    }

    void UpdateDash(float delta_t) {
        float direction = 0f;
        if (m_DashLeftAction.action.WasPressedThisFrame()) {
            direction = -1f;
        } else if (m_DashRightAction.action.WasPressedThisFrame()) {
            direction = 1f;
        }

        /*
        var dash_offset_target = m_DashForce * direction * (transform.right + 0.3f * transform.forward);
        if (direction != 0f && m_DashCoroutine == null) {
            m_DashCoroutine = StartCoroutine(DashCoroutine(1f, dash_offset_target));
        }
        //var offset_current = m_Kart.transform.localPosition - m_KartInitLocalPos;
            //+= delta * delta_t * 4f;
        m_Kart.transform.localPosition = m_KartInitLocalPos;// + m_DashOffsetTarget;
        //m_DashOffsetTarget *= 0.5f * delta_t;
        */
    }

    public IEnumerator DashCoroutine(float duration, Vector3 offset) {
        m_DashOffsetTarget = offset;
        yield return new WaitForSeconds(duration);
        m_DashOffsetTarget = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Track")) {
            return;
        }
        foreach (ContactPoint contact in collision.contacts) {
            float f = m_BounceOutOfBounds ? 1f : 0.4f;
            var force = 20f * f * (0.3f * collision.relativeVelocity.magnitude) * contact.normal / collision.contacts.Length;
            m_RigidBody.AddForce(force, ForceMode.Impulse);
            Debug.Log("hit wall");
        }
    }

}
