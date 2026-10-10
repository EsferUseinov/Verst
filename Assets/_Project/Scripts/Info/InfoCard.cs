using TMPro;
using UnityEngine;

public class InfoCard : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private float gapAbove = 0.06f;
    [SerializeField] private float hideDelay = 0.3f;

    public static InfoCard Current { get; private set; }

    private Transform head;
    private Transform target;
    private float topOffset;
    private float hideAt = -1f;
    private bool cardsEnabled = true;

    public bool CardsEnabled
    {
        get => cardsEnabled;
        set
        {
            cardsEnabled = value;
            if (!cardsEnabled)
            {
                HideNow();
            }
        }
    }

    private void Awake()
    {
        Current = this;
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Current == this)
        {
            Current = null;
        }
    }

    public void Show(Transform newTarget, ItemDefinition definition)
    {
        if (!cardsEnabled || newTarget == null || definition == null)
        {
            return;
        }

        target = newTarget;
        topOffset = GetTopOffset(target);
        hideAt = -1f;

        titleText.text = definition.DisplayName;
        categoryText.text = GetCategoryName(definition.Category);
        descriptionText.text = definition.Description;

        UpdatePose();
        panel.SetActive(true);
    }

    public void Hide(Transform requester)
    {
        if (requester == target)
        {
            hideAt = Time.time + hideDelay;
        }
    }

    private void HideNow()
    {
        target = null;
        hideAt = -1f;
        panel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!panel.activeSelf)
        {
            return;
        }

        if (target == null || (hideAt >= 0f && Time.time >= hideAt))
        {
            HideNow();
            return;
        }

        UpdatePose();
    }

    private void UpdatePose()
    {
        if (head == null && Camera.main != null)
        {
            head = Camera.main.transform;
        }

        transform.position = target.position + Vector3.up * (topOffset + gapAbove);

        if (head != null)
        {
            Vector3 direction = Vector3.ProjectOnPlane(transform.position - head.position, Vector3.up);
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }

    private static float GetTopOffset(Transform root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return 0f;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds.max.y - root.position.y;
    }

    private static string GetCategoryName(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Tool: return "Інструмент";
            case ItemCategory.Fastener: return "Кріплення";
            case ItemCategory.Part: return "Деталь";
            case ItemCategory.Container: return "Тара";
            case ItemCategory.Test: return "Тестовий предмет";
            case ItemCategory.Product: return "Виріб";
            default: return category.ToString();
        }
    }
}
