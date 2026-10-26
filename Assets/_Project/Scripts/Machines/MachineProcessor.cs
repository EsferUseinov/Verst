using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class MachineProcessor : MonoBehaviour
{
    [SerializeField] private MachineController machine;
    [SerializeField] private SnapInteractable slot;
    [SerializeField] private RecipeDefinition[] recipes;
    [SerializeField] private Transform outputPoint;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private float duration = 2f;
    [SerializeField] private string emptyMessage = "Покладіть сировину на лоток";
    [SerializeField] private string startHint = "натисніть ПУСК";
    [SerializeField] private string noRecipeMessage = "Не обробляється";
    [SerializeField] private string doneMessage = "Готово";
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color doneColor = new Color(0.2f, 0.8f, 0.3f);

    private Item workpiece;
    private RecipeDefinition recipe;
    private float progress;
    private string lastResult;
    private int shownPercent = -1;
    private bool shownRunning;

    private void OnEnable()
    {
        slot.WhenSelectingInteractorViewAdded += HandleSlotChanged;
        slot.WhenSelectingInteractorViewRemoved += HandleSlotChanged;
        Refresh();
    }

    private void OnDisable()
    {
        slot.WhenSelectingInteractorViewAdded -= HandleSlotChanged;
        slot.WhenSelectingInteractorViewRemoved -= HandleSlotChanged;
    }

    private void Update()
    {
        if (recipe == null)
        {
            return;
        }

        if (machine.IsRunning)
        {
            progress += Time.deltaTime * machine.SpeedFactor / duration;
            if (progress >= 1f)
            {
                Complete();
                return;
            }
        }

        int percent = Mathf.FloorToInt(progress * 100f);
        if (percent != shownPercent || machine.IsRunning != shownRunning)
        {
            ShowProgress(percent);
        }
    }

    private void HandleSlotChanged(IInteractorView interactor)
    {
        Refresh();
    }

    private void Refresh()
    {
        workpiece = null;
        foreach (SnapInteractor interactor in slot.SelectingInteractors)
        {
            workpiece = interactor.GetComponentInParent<Item>();
            if (workpiece != null)
            {
                break;
            }
        }

        recipe = FindRecipe(workpiece);
        progress = 0f;

        if (workpiece == null)
        {
            bool done = !string.IsNullOrEmpty(lastResult);
            SetText(done ? $"{doneMessage}: {lastResult}" : emptyMessage, done ? doneColor : normalColor);
        }
        else if (recipe == null)
        {
            SetText(noRecipeMessage, normalColor);
        }
        else
        {
            lastResult = null;
            ShowProgress(0);
        }
    }

    private void ShowProgress(int percent)
    {
        shownPercent = percent;
        shownRunning = machine.IsRunning;
        string title = $"{workpiece.Definition.DisplayName} -> {recipe.DisplayName}";
        SetText(shownRunning ? $"{title}: {percent}%" : $"{title}, {startHint}", normalColor);
    }

    private void Complete()
    {
        Item source = workpiece;
        RecipeDefinition done = recipe;
        lastResult = done.DisplayName;
        workpiece = null;
        recipe = null;

        SnapInteractor snapInteractor = source.GetComponentInChildren<SnapInteractor>();
        if (snapInteractor != null)
        {
            snapInteractor.enabled = false;
        }
        Destroy(source.gameObject);

        Item result = Instantiate(done.ResultPrefab, outputPoint.position, outputPoint.rotation);
        result.name = done.ResultPrefab.name;
        SetText($"{doneMessage}: {lastResult}", doneColor);
        WorkshopEvents.Raise("machined:" + done.Id);
    }

    private RecipeDefinition FindRecipe(Item item)
    {
        if (item == null)
        {
            return null;
        }

        foreach (RecipeDefinition candidate in recipes)
        {
            if (candidate.Ingredients.Count > 0 && candidate.Ingredients[0].Item == item.Definition)
            {
                return candidate;
            }
        }
        return null;
    }

    private void SetText(string text, Color color)
    {
        statusText.text = text;
        statusText.color = color;
    }
}
