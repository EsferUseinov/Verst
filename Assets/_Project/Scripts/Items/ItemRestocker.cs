using System;
using UnityEngine;

[Serializable]
public class RestockEntry
{
    public Item sceneItem;
    public Item prefab;
}

public class ItemRestocker : MonoBehaviour
{
    [SerializeField] private string restockEvent = "wizard_started:first_shift";
    [SerializeField] private RestockEntry[] entries = new RestockEntry[0];

    private Item[] current;
    private string[] names;
    private Transform[] parents;
    private Vector3[] positions;
    private Quaternion[] rotations;

    private void Awake()
    {
        int count = entries.Length;
        current = new Item[count];
        names = new string[count];
        parents = new Transform[count];
        positions = new Vector3[count];
        rotations = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            Item item = entries[i].sceneItem;
            current[i] = item;
            names[i] = item.name;
            parents[i] = item.transform.parent;
            positions[i] = item.transform.position;
            rotations[i] = item.transform.rotation;
        }
    }

    private void OnEnable()
    {
        WorkshopEvents.Raised += HandleEvent;
    }

    private void OnDisable()
    {
        WorkshopEvents.Raised -= HandleEvent;
    }

    private void HandleEvent(string id)
    {
        if (id == restockEvent)
        {
            Restock();
        }
    }

    public void Restock()
    {
        for (int i = 0; i < entries.Length; i++)
        {
            if (current[i] != null)
            {
                continue;
            }

            current[i] = Instantiate(entries[i].prefab, positions[i], rotations[i], parents[i]);
            current[i].name = names[i];
        }
    }
}
