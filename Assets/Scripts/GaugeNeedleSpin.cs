using UnityEngine;

public class GaugeNeedleSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
