using System.Collections;
using UnityEngine;

public class CraftButtonFeedback : MonoBehaviour
{
    [SerializeField] private Renderer cap;
    [SerializeField] private Color idleColor = new Color(0.45f, 0.1f, 0.1f);
    [SerializeField] private Color readyColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color craftedColor = new Color(1f, 0.8f, 0.1f);
    [SerializeField] private Color failedColor = new Color(1f, 0.15f, 0.1f);
    [SerializeField] private float flashDuration = 0.4f;

    private bool isReady;
    private Coroutine flash;

    public void SetReady(bool ready)
    {
        isReady = ready;
        if (flash == null)
        {
            SetColor(CurrentColor);
        }
    }

    public void ShowCrafted()
    {
        Flash(craftedColor);
    }

    public void ShowFailed()
    {
        Flash(failedColor);
    }

    private Color CurrentColor => isReady ? readyColor : idleColor;

    private void Flash(Color color)
    {
        if (flash != null)
        {
            StopCoroutine(flash);
        }

        flash = StartCoroutine(FlashRoutine(color));
    }

    private IEnumerator FlashRoutine(Color color)
    {
        SetColor(color);
        yield return new WaitForSeconds(flashDuration);
        flash = null;
        SetColor(CurrentColor);
    }

    private void SetColor(Color color)
    {
        cap.material.color = color;
    }
}
