using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class CraftingStation : MonoBehaviour
{
    [SerializeField] private SnapInteractable[] slots;
    [SerializeField] private RecipeDefinition[] recipes;
    [SerializeField] private Transform outputPoint;
    [SerializeField] private UnityEvent<bool> onRecipeReadyChanged;
    [SerializeField] private UnityEvent onCrafted;
    [SerializeField] private UnityEvent onCraftFailed;

    private readonly List<Item> ingredientsBuffer = new List<Item>();
    private bool isReady;

    private void OnEnable()
    {
        foreach (SnapInteractable slot in slots)
        {
            slot.WhenSelectingInteractorViewAdded += HandleSlotChanged;
            slot.WhenSelectingInteractorViewRemoved += HandleSlotChanged;
        }
    }

    private void OnDisable()
    {
        foreach (SnapInteractable slot in slots)
        {
            slot.WhenSelectingInteractorViewAdded -= HandleSlotChanged;
            slot.WhenSelectingInteractorViewRemoved -= HandleSlotChanged;
        }
    }

    private void Start()
    {
        isReady = FindRecipe(ingredientsBuffer) != null;
        onRecipeReadyChanged.Invoke(isReady);
    }

    public void TryCraft()
    {
        List<Item> ingredients = new List<Item>();
        RecipeDefinition recipe = FindRecipe(ingredients);
        if (recipe == null)
        {
            Debug.Log(name + ": no recipe matches the slot contents");
            onCraftFailed.Invoke();
            return;
        }

        foreach (Item item in ingredients)
        {
            SnapInteractor snapInteractor = item.GetComponentInChildren<SnapInteractor>();
            if (snapInteractor != null)
            {
                snapInteractor.enabled = false;
            }

            Destroy(item.gameObject);
        }

        Item result = Instantiate(recipe.ResultPrefab, outputPoint.position, outputPoint.rotation);
        result.name = recipe.ResultPrefab.name;
        Debug.Log(name + ": crafted " + recipe.DisplayName);

        onCrafted.Invoke();
        RefreshReady();
    }

    private void HandleSlotChanged(IInteractorView interactor)
    {
        RefreshReady();
    }

    private void RefreshReady()
    {
        bool ready = FindRecipe(ingredientsBuffer) != null;
        if (ready != isReady)
        {
            isReady = ready;
            onRecipeReadyChanged.Invoke(isReady);
        }
    }

    private RecipeDefinition FindRecipe(List<Item> ingredients)
    {
        List<Item> available = CollectSlotItems();

        foreach (RecipeDefinition recipe in recipes)
        {
            if (TryMatch(recipe, available, ingredients))
            {
                return recipe;
            }
        }

        ingredients.Clear();
        return null;
    }

    private List<Item> CollectSlotItems()
    {
        List<Item> items = new List<Item>();
        foreach (SnapInteractable slot in slots)
        {
            foreach (SnapInteractor interactor in slot.SelectingInteractors)
            {
                Item item = interactor.GetComponentInParent<Item>();
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    private static bool TryMatch(RecipeDefinition recipe, List<Item> available, List<Item> ingredients)
    {
        ingredients.Clear();
        if (recipe.Ingredients.Count == 0)
        {
            return false;
        }

        List<Item> pool = new List<Item>(available);
        foreach (RecipeIngredient ingredient in recipe.Ingredients)
        {
            for (int i = 0; i < ingredient.Count; i++)
            {
                int index = pool.FindIndex(item => item.Definition == ingredient.Item);
                if (index < 0)
                {
                    ingredients.Clear();
                    return false;
                }

                ingredients.Add(pool[index]);
                pool.RemoveAt(index);
            }
        }

        return true;
    }
}
