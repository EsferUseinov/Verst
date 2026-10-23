using System;
using System.Collections;
using System.Linq;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class AssemblyNode : MonoBehaviour
{
    [SerializeField] private string nodeId = "spindle";
    [SerializeField] private SnapInteractable partSlot;
    [SerializeField] private SnapInteractable fastenerSlot;
    [SerializeField] private GameObject fastenerMarker;
    [SerializeField] private Renderer statusLamp;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Color emptyColor = new Color(0.35f, 0.3f, 0.4f);
    [SerializeField] private Color partColor = new Color(1f, 0.8f, 0.1f);
    [SerializeField] private Color assembledColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private string emptyMessage = "Встановіть шестерню в паз";
    [SerializeField] private string partMessage = "Закріпіть шестерню болтом";
    [SerializeField] private string assembledMessage = "Вузол зібрано";
    [SerializeField] private float pulseScale = 1.6f;
    [SerializeField] private float pulseDuration = 0.4f;

    private Item lockedPart;
    private Vector3 lampScale;
    private Coroutine pulse;

    public bool IsAssembled { get; private set; }

    public event Action<bool> AssembledChanged;

    private void Awake()
    {
        lampScale = statusLamp.transform.localScale;
    }

    private void OnEnable()
    {
        partSlot.WhenSelectingInteractorViewAdded += HandleSlotChanged;
        partSlot.WhenSelectingInteractorViewRemoved += HandleSlotChanged;
        fastenerSlot.WhenSelectingInteractorViewAdded += HandleSlotChanged;
        fastenerSlot.WhenSelectingInteractorViewRemoved += HandleSlotChanged;
        Refresh();
    }

    private void OnDisable()
    {
        partSlot.WhenSelectingInteractorViewAdded -= HandleSlotChanged;
        partSlot.WhenSelectingInteractorViewRemoved -= HandleSlotChanged;
        fastenerSlot.WhenSelectingInteractorViewAdded -= HandleSlotChanged;
        fastenerSlot.WhenSelectingInteractorViewRemoved -= HandleSlotChanged;
        Lock(null);
        SetAssembled(false);
    }

    private void HandleSlotChanged(IInteractorView interactor)
    {
        Refresh();
    }

    private void Refresh()
    {
        bool hasPart = partSlot.SelectingInteractorViews.Any();
        bool hasFastener = fastenerSlot.SelectingInteractorViews.Any();

        fastenerSlot.enabled = hasPart || hasFastener;
        fastenerMarker.SetActive(fastenerSlot.enabled);

        bool assembled = hasPart && hasFastener;
        Lock(assembled ? FindItem(partSlot) : null);

        statusLamp.material.color = assembled ? assembledColor : hasPart ? partColor : emptyColor;
        statusText.text = assembled ? assembledMessage : hasPart ? partMessage : emptyMessage;
        statusText.color = assembled ? assembledColor : Color.white;
        SetAssembled(assembled);
    }

    private void SetAssembled(bool assembled)
    {
        if (IsAssembled == assembled)
        {
            return;
        }

        IsAssembled = assembled;
        WorkshopEvents.SetFact($"assembled:{nodeId}", assembled);
        AssembledChanged?.Invoke(assembled);

        if (assembled && isActiveAndEnabled)
        {
            if (pulse != null)
            {
                StopCoroutine(pulse);
            }
            pulse = StartCoroutine(Pulse());
        }
    }

    private void Lock(Item part)
    {
        if (lockedPart == part)
        {
            return;
        }

        SetGrabEnabled(lockedPart, true);
        lockedPart = part;
        SetGrabEnabled(lockedPart, false);
    }

    private static void SetGrabEnabled(Item item, bool enabled)
    {
        if (item == null)
        {
            return;
        }

        foreach (MonoBehaviour behaviour in item.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (behaviour is IInteractable)
            {
                behaviour.enabled = enabled;
            }
        }
    }

    private static Item FindItem(SnapInteractable slot)
    {
        foreach (IInteractorView interactor in slot.SelectingInteractorViews)
        {
            if (interactor is Component component)
            {
                return component.GetComponentInParent<Item>();
            }
        }
        return null;
    }

    private IEnumerator Pulse()
    {
        Transform lamp = statusLamp.transform;
        for (float t = 0f; t < pulseDuration; t += Time.deltaTime)
        {
            float k = Mathf.Sin(t / pulseDuration * Mathf.PI);
            lamp.localScale = lampScale * Mathf.Lerp(1f, pulseScale, k);
            yield return null;
        }
        lamp.localScale = lampScale;
        pulse = null;
    }
}
