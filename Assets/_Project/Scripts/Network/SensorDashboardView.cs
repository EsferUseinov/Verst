using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensorDashboardView : MonoBehaviour
{
    [SerializeField] private ApiSettings api;
    [SerializeField] private string sensorsPath = "/api/sensors";
    [SerializeField, Min(0f)] private float autoRefreshSec = 5f;
    [SerializeField] private TMP_Text machinesText;
    [SerializeField] private Button refreshButton;
    [SerializeField] private NetworkStatusView status;

    [Header("Machine status colors")]
    [SerializeField] private Color okColor = new Color(0.45f, 0.85f, 0.45f);
    [SerializeField] private Color warningColor = new Color(0.95f, 0.75f, 0.2f);
    [SerializeField] private Color alarmColor = new Color(0.9f, 0.3f, 0.25f);

    private bool loading;

    private void Awake()
    {
        refreshButton.onClick.AddListener(Refresh);
    }

    private void OnEnable()
    {
        status.Show(NetworkState.Idle);
        StartCoroutine(AutoRefresh());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        loading = false;
    }

    public void Refresh()
    {
        if (loading || !isActiveAndEnabled)
        {
            return;
        }
        StartCoroutine(Load());
    }

    private IEnumerator AutoRefresh()
    {
        while (true)
        {
            Refresh();
            if (autoRefreshSec <= 0f)
            {
                yield break;
            }
            yield return new WaitForSeconds(autoRefreshSec);
        }
    }

    private IEnumerator Load()
    {
        loading = true;
        refreshButton.interactable = false;
        status.Show(NetworkState.Loading);

        yield return ApiClient.Get<SensorSnapshot>(api, sensorsPath, ShowSnapshot, ShowError);

        refreshButton.interactable = true;
        loading = false;
    }

    private void ShowSnapshot(SensorSnapshot snapshot)
    {
        StringBuilder builder = new StringBuilder();
        foreach (MachineReading machine in snapshot.machines)
        {
            string color = ColorUtility.ToHtmlStringRGB(StatusColor(machine.status));
            builder.AppendLine($"<b>{machine.name}</b>  <color=#{color}>{StatusLabel(machine.status)}</color>");
            builder.AppendLine($"{machine.temperature:0.0} °C   {machine.rpm} об/хв   навантаження {machine.load * 100f:0}%");
        }
        machinesText.text = builder.ToString().TrimEnd();
        status.Show(NetworkState.Success, $"оновлено о {FormatTime(snapshot.updatedAt)}");
    }

    private void ShowError(string error)
    {
        status.Show(NetworkState.Error, error);
    }

    private Color StatusColor(string machineStatus)
    {
        return machineStatus switch
        {
            "alarm" => alarmColor,
            "warning" => warningColor,
            _ => okColor
        };
    }

    private static string StatusLabel(string machineStatus)
    {
        return machineStatus switch
        {
            "alarm" => "перегрів",
            "warning" => "увага",
            _ => "норма"
        };
    }

    private static string FormatTime(string isoTime)
    {
        return DateTime.TryParse(isoTime, out DateTime time) ? time.ToLocalTime().ToString("HH:mm:ss") : DateTime.Now.ToString("HH:mm:ss");
    }
}
