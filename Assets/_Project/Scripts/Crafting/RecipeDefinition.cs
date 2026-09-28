using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecipeIngredient
{
    [SerializeField] private ItemDefinition item;
    [SerializeField, Min(1)] private int count = 1;

    public ItemDefinition Item => item;
    public int Count => count;
}

[CreateAssetMenu(fileName = "Recipe_", menuName = "Verst/Recipe Definition")]
public class RecipeDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
    [SerializeField] private Item resultPrefab;

    public string Id => id;
    public string DisplayName => displayName;
    public IReadOnlyList<RecipeIngredient> Ingredients => ingredients;
    public Item ResultPrefab => resultPrefab;
}
