using TMPro;
using UnityEngine;

public class NumericKeypad : MonoBehaviour
{
    [SerializeField] private TMP_InputField target;

    public void Type(string digit)
    {
        if (target.characterLimit > 0 && target.text.Length >= target.characterLimit)
        {
            return;
        }
        target.text += digit;
    }

    public void Backspace()
    {
        if (target.text.Length > 0)
        {
            target.text = target.text.Substring(0, target.text.Length - 1);
        }
    }

    public void Clear()
    {
        target.text = "";
    }
}
