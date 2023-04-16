using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Yarn.Unity;

public class PlayerCar : MonoBehaviour {

    [SerializeField]
    private InputActionReference
        m_MoveAction,
        m_DashLeftAction,
        m_DashRightAction,
        m_DriftAction;

    [SerializeField]
    private float m_TurnSens;

    [SerializeField]
    private float m_Speed;
    [SerializeField]
    private InMemoryVariableStorage m_DialougeMemory;

    [SerializeField]
    private Transform m_Kart;

    private float m_MaxSpeed = 10f;

    public int DinoCoins = 0;

    [SerializeField]
    private float m_DashForce;

    [SerializeField]
    private bool m_BounceOutOfBounds = true;

    private Animator m_Animator;

    private Rigidbody m_RigidBody;
    private Camera m_Camera;
    private float m_RotAngle = 0f;

    private bool m_Sleeping = false;
    private Vector3 m_PreSleepVel;
    private Vector3 m_PreSleepAngVel;

    private float m_DriftOffset = 0f;
    private Vector3 m_DashOffsetTarget = Vector3.zero;
    private Vector3 m_KartInitLocalPos;
    private AudioSource m_AudioSource;
    private Coroutine m_DashCoroutine = null;
    private Coroutine m_BoostCoroutine = null;
    public float Velocity {
        get => m_RigidBody.velocity.magnitude;
    }

    public float SignedVelocity {
        get => Velocity * (GoingForward ? 1f : -1f);
    }

    public void Boost(float duration){
        m_AudioSource.Play();
        StartCoroutine(BoostCoroutine(duration));
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

    public void SetDialougeCoins(){
        m_DialougeMemory.SetValue("$DinoCoin", DinoCoins);
    }

    void Start() {
        m_Camera = FindObjectOfType<Camera>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_KartInitLocalPos = m_Kart.transform.localPosition;
        m_RotAngle = transform.rotation.eulerAngles.y;
        var anims = GetComponentsInChildren<Animator>();
        if (anims.Length != 1) {
            Debug.LogWarning($"Found {anims.Length} animators for {name}, expected one.  Please make m_Animator of PlayerCar a SerializeField.");
        }
        m_Animator = anims[0];
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

    void Update() {
        UpdateDash(Time.deltaTime);
    }

    void FixedUpdate() {
        if (GameManager.Instance.Frozen) {
            return;
        }
        var delta_t = Time.fixedDeltaTime;

        UpdateDrift(delta_t);

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
            target = Mathf.Sign(input) * 25f;
            if (Input.GetKeyDown(KeyCode.Space)) {
                m_RigidBody.AddForce(40f * Vector3.up, ForceMode.Impulse);
            }
        }
        var delta = target - m_DriftOffset;
        var offset_new = m_DriftOffset + (delta * delta_t * 4f);
        if (Mathf.Sign(m_DriftOffset) + Mathf.Sign(offset_new) != 0) {
            m_DriftOffset = offset_new;
        } else {
            m_DriftOffset = target;
        }
    }

    void UpdateDash(float delta_t) {
        if (m_DashLeftAction.action.WasPressedThisFrame()) {
            m_Animator.SetTrigger("DashLeft");
            m_RigidBody.AddForce(30f * -(transform.right) , ForceMode.Impulse);
        } else if (m_DashRightAction.action.WasPressedThisFrame()) {
            m_Animator.SetTrigger("DashRight");
            m_RigidBody.AddForce(30f * transform.right, ForceMode.Impulse);
        }
    }
    public IEnumerator BoostCoroutine(float duration) {
        m_RigidBody.AddForce(transform.forward* 50f, ForceMode.Impulse );
        yield return new WaitForSeconds(duration);
    }


    public bool IsDashing() {
        return m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Gocart|CarDashLeft") ||
            m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Gocart|CarDashRight");
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
