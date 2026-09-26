using UnityEngine;

public enum ItemCategory
{
    Tool,
    Fastener,
    Part,
    Container,
    Test
}

[CreateAssetMenu(fileName = "Item_", menuName = "Verst/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private ItemCategory category;

    public string Id => id;
    public string DisplayName => displayName;
    public ItemCategory Category => category;
}
