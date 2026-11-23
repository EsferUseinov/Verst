using UnityEngine;

public enum ItemCategory
{
    Tool,
    Fastener,
    Part,
    Container,
    Test,
    Product,
    Raw
}

[CreateAssetMenu(fileName = "Item_", menuName = "Verst/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private ItemCategory category;
    [SerializeField, TextArea] private string description;
    [SerializeField] private BattleEffect battleEffect;

    public string Id => id;
    public string DisplayName => displayName;
    public ItemCategory Category => category;
    public string Description => description;
    public BattleEffect BattleEffect => battleEffect;
}
