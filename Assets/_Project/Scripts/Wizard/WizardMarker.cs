using TMPro;
using UnityEngine;

public class WizardMarker : MonoBehaviour
{
    [SerializeField] private string markerId;
    [SerializeField] private GameObject visual;
    [SerializeField] private Transform bobber;
    [SerializeField] private Transform label;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private float bobHeight = 0.03f;
    [SerializeField] private float bobSpeed = 2.5f;
    [SerializeField] private float spinSpeed = 90f;

    public string MarkerId => markerId;

    private Transform head;
    private Vector3 bobberStart;

    private void Awake()
    {
        bobberStart = bobber.localPosition;
        visual.SetActive(false);
    }

    public void Show(string text)
    {
        labelText.text = text;
        visual.SetActive(true);
    }

    public void Hide()
    {
        visual.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!visual.activeSelf)
        {
            return;
        }

        bobber.localPosition = bobberStart + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        bobber.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

        if (head == null && Camera.main != null)
        {
            head = Camera.main.transform;
        }

        if (head != null)
        {
            Vector3 direction = Vector3.ProjectOnPlane(label.position - head.position, Vector3.up);
            if (direction.sqrMagnitude > 0.0001f)
            {
                label.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }
}
