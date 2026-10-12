using UnityEngine;

public class WizardTabletFocus : MonoBehaviour
{
    [SerializeField] private WizardController controller;
    [SerializeField] private TabletPresenter tablet;
    [SerializeField] private TabView tabView;
    [SerializeField] private int wizardPage = 2;

    private void OnEnable()
    {
        controller.Changed += HandleChanged;
    }

    private void OnDisable()
    {
        controller.Changed -= HandleChanged;
    }

    private void HandleChanged()
    {
        bool needsTablet = controller.State == WizardState.Result
            || (controller.State == WizardState.Step && controller.CurrentStep.Type == WizardStepType.SingleChoice);
        if (!needsTablet)
        {
            return;
        }

        tablet.Show();
        tabView.ShowPage(wizardPage);
    }
}
