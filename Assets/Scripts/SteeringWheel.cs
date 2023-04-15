using UnityEngine;

public class SteeringWheel : MonoBehaviour {
    [SerializeField]
    private PlayerCar m_Car;

    [SerializeField]
    private float m_MaxAngle = 150f;

    [SerializeField]
    private Vector3 m_Axis = new Vector3(1, 0, 0);

    private float m_TurnSpeed = 4f;
    private float m_TargetAngle = 0f;
    private float m_Angle = 0f;
    private Quaternion m_InitialRotation;

    void Start() {
        m_InitialRotation = transform.rotation;
    }

    void Update() {
        float angle_prev = m_Angle;
        m_TargetAngle = -1f * m_MaxAngle * m_Car.SteeringInput;
        var delta = m_TargetAngle - m_Angle;
        m_Angle += delta * Time.deltaTime * m_TurnSpeed;
        //transform.rotation = m_InitialRotation * Quaternion.Euler(m_Angle * new Vector3(1, 0, 0));
        transform.Rotate((m_Angle - angle_prev) * m_Axis);
    }
}
