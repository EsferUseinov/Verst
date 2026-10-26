using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PokeToggleSwitch : MonoBehaviour
{
    [SerializeField, Interface(typeof(IInteractableView))] private Object interactable;
    [SerializeField] private Transform lever;
    [SerializeField] private float offAngle = -25f;
    [SerializeField] private float onAngle = 25f;
    [SerializeField] private Renderer indicator;
    [SerializeField] private Color offColor = new Color(0.25f, 0.2f, 0.3f);
    [SerializeField] private Color onColor = new Color(0.949f, 0.549f, 0.157f);
    [SerializeField] private bool isOn;
    [SerializeField] private UnityEvent<bool> onChanged = new UnityEvent<bool>();

    private IInteractableView view;

    public bool IsOn => isOn;

    private void Awake()
    {
        view = interactable as IInteractableView;
    }

    private void OnEnable()
    {
        view.WhenStateChanged += HandleStateChanged;
        Apply();
    }

    private void OnDisable()
    {
        view.WhenStateChanged -= HandleStateChanged;
    }

    public void SetOn(bool value)
    {
        if (isOn == value)
        {
            return;
        }

        isOn = value;
        Apply();
        onChanged.Invoke(isOn);
    }

    private void HandleStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            SetOn(!isOn);
        }
    }

    private void Apply()
    {
        lever.localRotation = Quaternion.Euler(isOn ? onAngle : offAngle, 0f, 0f);
        indicator.material.color = isOn ? onColor : offColor;
    }
}
