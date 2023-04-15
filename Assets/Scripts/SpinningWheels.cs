using UnityEngine;

public class SpinningWheels : MonoBehaviour {
    [SerializeField]
    private PlayerCar m_Car;

    private float m_TurnSpeed = 80f;

    void Update() {
        var theta = -1f * Time.deltaTime * m_TurnSpeed * m_Car.SignedVelocity;
        transform.Rotate(new Vector3(0, 1, 0), theta);
    }
}
