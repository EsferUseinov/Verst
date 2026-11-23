using Oculus.Interaction;
using UnityEngine;

public class InteractableColorFeedback : MonoBehaviour
{
    [SerializeField, Interface(typeof(IInteractableView))] private Object interactable;
    [SerializeField, Interface(typeof(IInteractableView)), Optional] private Object rayInteractable;
    [SerializeField] private Renderer target;
    [SerializeField] private Color normalColor = new Color(0.424f, 0.29f, 0.714f);
    [SerializeField] private Color hoverColor = new Color(0.557f, 0.42f, 0.878f);
    [SerializeField] private Color selectColor = new Color(0.949f, 0.549f, 0.157f);

    private IInteractableView view;
    private IInteractableView rayView;

    private void Awake()
    {
        view = interactable as IInteractableView;
        rayView = rayInteractable as IInteractableView;
    }

    private void OnEnable()
    {
        view.WhenStateChanged += HandleStateChanged;
        if (rayView != null)
        {
            rayView.WhenStateChanged += HandleStateChanged;
        }
        Refresh();
    }

    private void OnDisable()
    {
        view.WhenStateChanged -= HandleStateChanged;
        if (rayView != null)
        {
            rayView.WhenStateChanged -= HandleStateChanged;
        }
    }

    private void HandleStateChanged(InteractableStateChangeArgs args)
    {
        Refresh();
    }

    private void Refresh()
    {
        InteractableState rayState = rayView != null ? rayView.State : InteractableState.Normal;
        bool selected = view.State == InteractableState.Select || rayState == InteractableState.Select;
        bool hovered = view.State == InteractableState.Hover || rayState == InteractableState.Hover;
        target.material.color = selected ? selectColor : hovered ? hoverColor : normalColor;
    }
}
