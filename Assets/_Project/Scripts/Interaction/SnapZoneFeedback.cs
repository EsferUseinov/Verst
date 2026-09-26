using UnityEngine;

public class SnapZoneFeedback : MonoBehaviour
{
    [SerializeField] private Renderer marker;
    [SerializeField] private Color idleColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0.1f);
    [SerializeField] private Color occupiedColor = new Color(0.2f, 0.8f, 0.3f);

    private void Start()
    {
        ShowIdle();
    }

    public void ShowIdle()
    {
        SetColor(idleColor);
    }

    public void ShowHover()
    {
        SetColor(hoverColor);
    }

    public void ShowOccupied()
    {
        SetColor(occupiedColor);
        Debug.Log(name + ": item snapped");
    }

    private void SetColor(Color color)
    {
        marker.material.color = color;
    }
}
