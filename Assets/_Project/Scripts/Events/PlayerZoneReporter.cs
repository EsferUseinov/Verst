using UnityEngine;

public class PlayerZoneReporter : MonoBehaviour
{
    [SerializeField] private string factId = "at:assembly";
    [SerializeField] private float radius = 1.2f;

    private Transform head;
    private bool inside;

    private void Update()
    {
        if (head == null)
        {
            if (Camera.main == null)
            {
                return;
            }
            head = Camera.main.transform;
        }

        Vector3 offset = head.position - transform.position;
        offset.y = 0f;
        SetInside(offset.sqrMagnitude <= radius * radius);
    }

    private void OnDisable()
    {
        SetInside(false);
    }

    private void SetInside(bool value)
    {
        if (inside == value)
        {
            return;
        }
        inside = value;
        WorkshopEvents.SetFact(factId, value);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
