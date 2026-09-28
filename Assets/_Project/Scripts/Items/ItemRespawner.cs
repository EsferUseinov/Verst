using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemRespawner : MonoBehaviour
{
    [SerializeField] private float minHeight = -2f;

    private Rigidbody body;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (body.position.y < minHeight)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        Debug.Log(name + " respawned");
    }
}
