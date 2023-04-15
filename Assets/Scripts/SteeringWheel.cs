using UnityEngine;

public class SteeringWheel : MonoBehaviour {
    [SerializeField]
    private PlayerCar m_Car;

    private float m_MaxAngle = 50f;
    private float m_TargetAngle = 0f;
    private float m_Angle = 0f;
    private Quaternion m_InitialRotation;

    void Start() {
        m_InitialRotation = transform.rotation;
    }

    void Update() {
        m_TargetAngle = m_MaxAngle * m_Car.SteeringInput;
        var delta = m_TargetAngle - m_Angle;
        m_Angle += -1f * delta * Time.deltaTime * 3f;
        transform.rotation = m_InitialRotation * Quaternion.Euler(m_Angle * new Vector3(1, 0, 0));
    }
}
