using UnityEngine;
using UnityEngine.Events;

public class MenuButtonInput : MonoBehaviour
{
    [SerializeField] private OVRInput.Button button = OVRInput.Button.Start;
    [SerializeField] private UnityEvent onPressed;

    private void Update()
    {
        if (OVRInput.GetDown(button))
        {
            onPressed.Invoke();
        }
    }
}
