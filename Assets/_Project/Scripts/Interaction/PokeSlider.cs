using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PokeSlider : MonoBehaviour
{
    [SerializeField, Interface(typeof(IPointable))] private Object pointable;
    [SerializeField] private Transform knob;
    [SerializeField] private float halfLength = 0.08f;
    [SerializeField] private float step = 0.1f;
    [SerializeField, Range(0f, 1f)] private float value = 0.5f;
    [SerializeField] private UnityEvent<float> onValueChanged = new UnityEvent<float>();

    private readonly HashSet<int> selecting = new HashSet<int>();
    private IPointable source;

    public float Value => value;

    private void Awake()
    {
        source = pointable as IPointable;
    }

    private void OnEnable()
    {
        source.WhenPointerEventRaised += HandlePointerEvent;
        ApplyVisual();
    }

    private void OnDisable()
    {
        source.WhenPointerEventRaised -= HandlePointerEvent;
        selecting.Clear();
    }

    private void Start()
    {
        onValueChanged.Invoke(value);
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                selecting.Add(evt.Identifier);
                UpdateFrom(evt.Pose.position);
                break;
            case PointerEventType.Move:
                if (selecting.Contains(evt.Identifier))
                {
                    UpdateFrom(evt.Pose.position);
                }
                break;
            case PointerEventType.Unselect:
            case PointerEventType.Cancel:
                selecting.Remove(evt.Identifier);
                break;
        }
    }

    private void UpdateFrom(Vector3 worldPoint)
    {
        float local = transform.InverseTransformPoint(worldPoint).x;
        float newValue = Mathf.InverseLerp(-halfLength, halfLength, local);
        if (step > 0f)
        {
            newValue = Mathf.Round(newValue / step) * step;
        }

        if (Mathf.Approximately(newValue, value))
        {
            return;
        }

        value = newValue;
        ApplyVisual();
        onValueChanged.Invoke(value);
    }

    private void ApplyVisual()
    {
        Vector3 position = knob.localPosition;
        position.x = Mathf.Lerp(-halfLength, halfLength, value);
        knob.localPosition = position;
    }
}
