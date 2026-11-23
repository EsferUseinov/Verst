using TMPro;
using UnityEngine;

public class RawSupply : MonoBehaviour
{
    [SerializeField, Min(0)] private int startAmount = 6;
    [SerializeField, Min(1)] private int capacity = 6;
    [SerializeField, Min(0)] private int perDelivery = 4;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1f, 0.15f, 0.1f);

    public int Amount { get; private set; }

    private void Awake()
    {
        Refill();
    }

    public bool TryTake()
    {
        if (Amount <= 0)
        {
            return false;
        }

        Amount--;
        UpdateLabel();
        return true;
    }

    public void Deliver()
    {
        Amount = Mathf.Min(capacity, Amount + perDelivery);
        UpdateLabel();
    }

    public void Refill()
    {
        Amount = Mathf.Min(startAmount, capacity);
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        label.text = $"Сировина: {Amount} / {capacity}";
        label.color = Amount > 0 ? normalColor : emptyColor;
    }
}
