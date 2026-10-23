using System.Collections;
using TMPro;
using UnityEngine;

public class MachineController : MonoBehaviour
{
    [SerializeField] private string machineId = "machine_1";
    [SerializeField] private AssemblyNode requiredNode;
    [SerializeField] private Transform spindle;
    [SerializeField] private Renderer[] lamps;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private float maxSpeed = 720f;
    [SerializeField] private float acceleration = 540f;
    [SerializeField] private Color offColor = new Color(0.25f, 0.2f, 0.3f);
    [SerializeField] private Color poweredColor = new Color(1f, 0.8f, 0.1f);
    [SerializeField] private Color runningColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color errorColor = new Color(1f, 0.15f, 0.1f);
    [SerializeField] private string offMessage = "Вимкнено";
    [SerializeField] private string readyMessage = "Готовий до пуску";
    [SerializeField] private string runningMessage = "Працює";
    [SerializeField] private string noPowerMessage = "Немає живлення";
    [SerializeField] private string notAssembledMessage = "Вузол не зібрано";
    [SerializeField] private float errorDuration = 1.5f;

    private bool powered;
    private bool running;
    private float speed = 0.5f;
    private float currentSpeed;
    private Coroutine error;

    private void OnEnable()
    {
        if (requiredNode != null)
        {
            requiredNode.AssembledChanged += HandleAssembledChanged;
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (requiredNode != null)
        {
            requiredNode.AssembledChanged -= HandleAssembledChanged;
        }
        SetRunning(false);
    }

    private void Update()
    {
        float target = running ? speed * maxSpeed : 0f;
        currentSpeed = Mathf.MoveTowards(currentSpeed, target, acceleration * Time.deltaTime);
        if (currentSpeed > 0f)
        {
            spindle.Rotate(0f, currentSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }

    public void SetPower(bool value)
    {
        powered = value;
        WorkshopEvents.SetFact($"powered:{machineId}", value);
        if (!powered)
        {
            SetRunning(false);
        }
        Refresh();
    }

    public void SetSpeed(float value)
    {
        speed = value;
        Refresh();
    }

    public void StartMachine()
    {
        if (!powered)
        {
            ShowError(noPowerMessage);
            return;
        }

        if (requiredNode != null && !requiredNode.IsAssembled)
        {
            ShowError(notAssembledMessage);
            return;
        }

        SetRunning(true);
        Refresh();
    }

    public void StopMachine()
    {
        SetRunning(false);
        Refresh();
    }

    private void HandleAssembledChanged(bool assembled)
    {
        if (!assembled && running)
        {
            SetRunning(false);
            ShowError(notAssembledMessage);
        }
    }

    private void SetRunning(bool value)
    {
        if (running == value)
        {
            return;
        }

        running = value;
        WorkshopEvents.SetFact($"running:{machineId}", value);
    }

    private void ShowError(string message)
    {
        if (error != null)
        {
            StopCoroutine(error);
        }
        error = StartCoroutine(ErrorRoutine(message));
    }

    private IEnumerator ErrorRoutine(string message)
    {
        SetLamps(errorColor);
        statusText.text = message;
        statusText.color = errorColor;
        yield return new WaitForSeconds(errorDuration);
        error = null;
        Refresh();
    }

    private void Refresh()
    {
        if (error != null)
        {
            return;
        }

        int percent = Mathf.RoundToInt(speed * 100f);
        if (running)
        {
            SetLamps(runningColor);
            statusText.text = $"{runningMessage} – {percent}%";
            statusText.color = runningColor;
        }
        else if (powered)
        {
            SetLamps(poweredColor);
            statusText.text = $"{readyMessage} – {percent}%";
            statusText.color = poweredColor;
        }
        else
        {
            SetLamps(offColor);
            statusText.text = offMessage;
            statusText.color = Color.white;
        }
    }

    private void SetLamps(Color color)
    {
        foreach (Renderer lamp in lamps)
        {
            lamp.material.color = color;
        }
    }
}
