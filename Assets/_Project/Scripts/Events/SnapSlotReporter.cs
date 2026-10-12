using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class SnapSlotReporter : MonoBehaviour
{
    [SerializeField] private SnapInteractable slot;
    [SerializeField] private string slotId;

    private readonly Dictionary<int, string> placed = new Dictionary<int, string>();

    private void OnEnable()
    {
        slot.WhenSelectingInteractorViewAdded += HandleAdded;
        slot.WhenSelectingInteractorViewRemoved += HandleRemoved;
    }

    private void OnDisable()
    {
        slot.WhenSelectingInteractorViewAdded -= HandleAdded;
        slot.WhenSelectingInteractorViewRemoved -= HandleRemoved;

        foreach (string factId in placed.Values)
        {
            WorkshopEvents.SetFact(factId, false);
        }
        placed.Clear();
    }

    private void HandleAdded(IInteractorView interactor)
    {
        if (!(interactor is Component component))
        {
            return;
        }

        Item item = component.GetComponentInParent<Item>();
        if (item == null || item.Definition == null)
        {
            return;
        }

        string factId = $"placed:{slotId}:{item.Definition.Id}";
        placed[interactor.Identifier] = factId;
        WorkshopEvents.SetFact(factId, true);
    }

    private void HandleRemoved(IInteractorView interactor)
    {
        if (placed.TryGetValue(interactor.Identifier, out string factId))
        {
            placed.Remove(interactor.Identifier);
            WorkshopEvents.SetFact(factId, false);
        }
    }
}
