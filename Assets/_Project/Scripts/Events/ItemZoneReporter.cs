using System.Collections.Generic;
using UnityEngine;

public class ItemZoneReporter : MonoBehaviour
{
    [SerializeField] private string zoneId = "qc";
    [SerializeField] private Vector3 size = Vector3.one;
    [SerializeField] private float interval = 0.2f;

    private readonly Dictionary<Item, string> inside = new Dictionary<Item, string>();
    private readonly HashSet<Item> found = new HashSet<Item>();
    private readonly List<Item> left = new List<Item>();
    private readonly Collider[] hits = new Collider[32];
    private float nextCheck;

    private void Update()
    {
        if (Time.time < nextCheck)
        {
            return;
        }
        nextCheck = Time.time + interval;
        Check();
    }

    private void OnDisable()
    {
        foreach (string factId in inside.Values)
        {
            WorkshopEvents.SetFact(factId, false);
        }
        inside.Clear();
    }

    private void Check()
    {
        found.Clear();
        int count = Physics.OverlapBoxNonAlloc(transform.position, size * 0.5f, hits, transform.rotation, Physics.AllLayers, QueryTriggerInteraction.Collide);
        for (int i = 0; i < count; i++)
        {
            Item item = hits[i].GetComponentInParent<Item>();
            if (item != null && item.Definition != null)
            {
                found.Add(item);
            }
        }

        foreach (Item item in found)
        {
            if (!inside.ContainsKey(item))
            {
                string factId = $"in:{zoneId}:{item.Definition.Id}";
                inside[item] = factId;
                WorkshopEvents.SetFact(factId, true);
            }
        }

        left.Clear();
        foreach (Item item in inside.Keys)
        {
            if (item == null || !found.Contains(item))
            {
                left.Add(item);
            }
        }

        foreach (Item item in left)
        {
            WorkshopEvents.SetFact(inside[item], false);
            inside.Remove(item);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
