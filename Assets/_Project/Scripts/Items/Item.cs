using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemDefinition definition;

    public ItemDefinition Definition => definition;
}
