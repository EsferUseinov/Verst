using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDispenser : MonoBehaviour
{
    [SerializeField] private Item prefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField, Min(1)] private int limit = 3;
    [SerializeField] private float stackStep = 0.08f;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color limitColor = new Color(1f, 0.15f, 0.1f);

    private readonly List<Item> spawned = new List<Item>();
    private int shownCount = -1;

    private void Update()
    {
        int count = Count();
        if (count != shownCount)
        {
            UpdateLabel(count);
        }
    }

    public void Dispense()
    {
        int count = Count();
        if (count >= limit)
        {
            return;
        }

        Vector3 position = spawnPoint.position + Vector3.up * (stackStep * count);
        Item item = Instantiate(prefab, position, spawnPoint.rotation);
        item.name = prefab.name;
        spawned.Add(item);
        UpdateLabel(spawned.Count);
        WorkshopEvents.Raise("dispensed:" + prefab.Definition.Id);
    }

    private int Count()
    {
        spawned.RemoveAll(item => item == null);
        return spawned.Count;
    }

    private void UpdateLabel(int count)
    {
        shownCount = count;
        label.text = $"{prefab.Definition.DisplayName}\n{count}/{limit}";
        label.color = count >= limit ? limitColor : normalColor;
    }
}
