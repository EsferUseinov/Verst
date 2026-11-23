using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ReportResponse
{
    public string id;
    public string receivedAt;
}

public class WizardReportSender : MonoBehaviour
{
    [SerializeField] private WizardController wizard;
    [SerializeField] private ApiSettings api;
    [SerializeField] private string reportsPath = "/api/reports";
    [SerializeField] private NetworkStatusView status;
    [SerializeField] private Button retryButton;

    private WizardResult pending;
    private bool sending;

    private void Awake()
    {
        retryButton.onClick.AddListener(Retry);
        retryButton.gameObject.SetActive(false);
        status.Show(NetworkState.Idle);
    }

    private void OnEnable()
    {
        wizard.Finished += Send;
    }

    private void OnDisable()
    {
        wizard.Finished -= Send;
    }

    private void Send(WizardResult result)
    {
        pending = result;
        StartCoroutine(Post());
    }

    private void Retry()
    {
        if (pending != null && !sending)
        {
            StartCoroutine(Post());
        }
    }

    private IEnumerator Post()
    {
        sending = true;
        retryButton.gameObject.SetActive(false);
        status.Show(NetworkState.Loading, "надсилання звіту");

        yield return ApiClient.PostJson<ReportResponse>(api, reportsPath, pending, OnSent, OnFailed);

        sending = false;
    }

    private void OnSent(ReportResponse response)
    {
        Debug.Log($"WizardReportSender: report {response.id} accepted at {response.receivedAt}");
        pending = null;
        status.Show(NetworkState.Success, $"звіт №{response.id} надіслано");
    }

    private void OnFailed(string error)
    {
        status.Show(NetworkState.Error, error);
        retryButton.gameObject.SetActive(true);
    }
}
