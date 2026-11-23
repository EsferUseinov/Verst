using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipmentZone : MonoBehaviour
{
    [SerializeField] private Vector3 size = new Vector3(1.2f, 0.6f, 0.8f);
    [SerializeField] private float interval = 0.25f;

    private readonly List<Item> cargo = new List<Item>();
    private readonly List<Item> found = new List<Item>();
    private readonly Collider[] hits = new Collider[64];
    private float nextCheck;

    public event Action Changed;

    public IReadOnlyList<Item> Cargo => cargo;

    private void Update()
    {
        if (Time.time < nextCheck)
        {
            return;
        }
        nextCheck = Time.time + interval;
        Scan();
    }

    public List<ItemDefinition> TakeCargo()
    {
        Scan();
        List<ItemDefinition> taken = new List<ItemDefinition>();
        foreach (Item item in cargo)
        {
            taken.Add(item.Definition);
            Destroy(item.gameObject);
        }
        cargo.Clear();
        Changed?.Invoke();
        return taken;
    }

    private void Scan()
    {
        found.Clear();
        int count = Physics.OverlapBoxNonAlloc(transform.position, size * 0.5f, hits, transform.rotation, Physics.AllLayers, QueryTriggerInteraction.Collide);
        for (int i = 0; i < count; i++)
        {
            Item item = hits[i].GetComponentInParent<Item>();
            if (item != null && item.Definition != null && item.Definition.BattleEffect != BattleEffect.None && !found.Contains(item))
            {
                found.Add(item);
            }
        }

        if (SameItems(found, cargo))
        {
            return;
        }

        cargo.Clear();
        cargo.AddRange(found);
        Changed?.Invoke();
    }

    private static bool SameItems(List<Item> a, List<Item> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        foreach (Item item in a)
        {
            if (!b.Contains(item))
            {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
