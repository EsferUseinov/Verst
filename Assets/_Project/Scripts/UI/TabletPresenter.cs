using UnityEngine;

public class TabletPresenter : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform head;
    [SerializeField, Range(0.35f, 0.6f)] private float distance = 0.45f;
    [SerializeField] private float heightOffset = -0.3f;
    [SerializeField] private float tiltAngle = 30f;
    [SerializeField] private bool openOnStart;

    public bool IsOpen => panel.activeSelf;

    public float Distance
    {
        get => distance;
        set => distance = Mathf.Clamp(value, 0.35f, 0.6f);
    }

    private void Start()
    {
        if (head == null && Camera.main != null)
        {
            head = Camera.main.transform;
        }

        if (openOnStart)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Toggle()
    {
        if (IsOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        PlaceInFrontOfHead();
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void PlaceInFrontOfHead()
    {
        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.001f)
        {
            Vector3 up = head.forward.y < 0f ? head.up : -head.up;
            forward = Vector3.ProjectOnPlane(up, Vector3.up);
        }
        forward.Normalize();

        transform.position = head.position + forward * distance + Vector3.up * heightOffset;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(tiltAngle, 0f, 0f);
    }
}
