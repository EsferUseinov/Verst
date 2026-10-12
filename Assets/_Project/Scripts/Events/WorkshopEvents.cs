using System;
using System.Collections.Generic;
using UnityEngine;

public static class WorkshopEvents
{
    private static readonly Dictionary<string, int> facts = new Dictionary<string, int>();

    public static event Action<string> Raised;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        facts.Clear();
        Raised = null;
    }

    public static void Raise(string id)
    {
        Raised?.Invoke(id);
    }

    public static void SetFact(string id, bool value)
    {
        facts.TryGetValue(id, out int count);
        int newCount = value ? count + 1 : Mathf.Max(0, count - 1);
        facts[id] = newCount;
        if (count == 0 && newCount > 0)
        {
            Raise(id);
        }
    }

    public static bool IsFact(string id)
    {
        return facts.TryGetValue(id, out int count) && count > 0;
    }
}
