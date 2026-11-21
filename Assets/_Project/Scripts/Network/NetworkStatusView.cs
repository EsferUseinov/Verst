using TMPro;
using UnityEngine;

public enum NetworkState
{
    Idle,
    Loading,
    Success,
    Error
}

public class NetworkStatusView : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Color idleColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color loadingColor = new Color(0.95f, 0.55f, 0.16f);
    [SerializeField] private Color successColor = new Color(0.45f, 0.85f, 0.45f);
    [SerializeField] private Color errorColor = new Color(0.9f, 0.3f, 0.25f);

    public NetworkState State { get; private set; }

    public void Show(NetworkState state, string detail = "")
    {
        State = state;
        string label = state switch
        {
            NetworkState.Loading => "Завантаження...",
            NetworkState.Success => "Успіх",
            NetworkState.Error => "Помилка з'єднання",
            _ => "Немає даних"
        };

        statusText.text = string.IsNullOrEmpty(detail) ? label : $"{label}: {detail}";
        statusText.color = state switch
        {
            NetworkState.Loading => loadingColor,
            NetworkState.Success => successColor,
            NetworkState.Error => errorColor,
            _ => idleColor
        };
    }
}
