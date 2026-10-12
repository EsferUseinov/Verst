using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class InfoCardTarget : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField, Interface(typeof(IInteractableView))] private List<Object> interactables = new List<Object>();

    private readonly List<IInteractableView> views = new List<IInteractableView>();
    private bool shown;

    private void Awake()
    {
        foreach (Object interactable in interactables)
        {
            if (interactable is IInteractableView view)
            {
                views.Add(view);
            }
        }
    }

    private void OnEnable()
    {
        foreach (IInteractableView view in views)
        {
            view.WhenStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        foreach (IInteractableView view in views)
        {
            view.WhenStateChanged -= HandleStateChanged;
        }
        SetShown(false);
    }

    private void HandleStateChanged(InteractableStateChangeArgs args)
    {
        bool active = false;
        foreach (IInteractableView view in views)
        {
            if (view.State == InteractableState.Hover || view.State == InteractableState.Select)
            {
                active = true;
                break;
            }
        }
        SetShown(active);
    }

    private void SetShown(bool value)
    {
        if (shown == value)
        {
            return;
        }
        shown = value;

        if (shown && item.Definition != null)
        {
            WorkshopEvents.Raise("hovered:" + item.Definition.Id);
        }

        InfoCard card = InfoCard.Current;
        if (card == null)
        {
            return;
        }

        if (shown)
        {
            card.Show(transform, item.Definition);
        }
        else
        {
            card.Hide(transform);
        }
    }
}
