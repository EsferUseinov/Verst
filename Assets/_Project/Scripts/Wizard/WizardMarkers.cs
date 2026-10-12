using UnityEngine;

public class WizardMarkers : MonoBehaviour
{
    [SerializeField] private WizardController controller;

    private WizardMarker[] markers;

    private void Awake()
    {
        markers = GetComponentsInChildren<WizardMarker>(true);
    }

    private void OnEnable()
    {
        controller.Changed += Refresh;
    }

    private void OnDisable()
    {
        controller.Changed -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        string markerId = null;
        string text = null;
        if (controller.State == WizardState.Step)
        {
            markerId = controller.CurrentStep.MarkerId;
            text = controller.CurrentStep.Title;
        }

        foreach (WizardMarker marker in markers)
        {
            if (!string.IsNullOrEmpty(markerId) && marker.MarkerId == markerId)
            {
                marker.Show(text);
            }
            else
            {
                marker.Hide();
            }
        }
    }
}
