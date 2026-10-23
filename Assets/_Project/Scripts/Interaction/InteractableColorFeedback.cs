using Oculus.Interaction;
using UnityEngine;

public class InteractableColorFeedback : MonoBehaviour
{
    [SerializeField, Interface(typeof(IInteractableView))] private Object interactable;
    [SerializeField] private Renderer target;
    [SerializeField] private Color normalColor = new Color(0.424f, 0.29f, 0.714f);
    [SerializeField] private Color hoverColor = new Color(0.557f, 0.42f, 0.878f);
    [SerializeField] private Color selectColor = new Color(0.949f, 0.549f, 0.157f);

    private IInteractableView view;

    private void Awake()
    {
        view = interactable as IInteractableView;
    }

    private void OnEnable()
    {
        view.WhenStateChanged += HandleStateChanged;
        Apply(view.State);
    }

    private void OnDisable()
    {
        view.WhenStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(InteractableStateChangeArgs args)
    {
        Apply(args.NewState);
    }

    private void Apply(InteractableState state)
    {
        target.material.color = state == InteractableState.Select ? selectColor
            : state == InteractableState.Hover ? hoverColor : normalColor;
    }
}
