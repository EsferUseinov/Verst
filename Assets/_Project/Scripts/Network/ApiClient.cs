using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiClient
{
    public static IEnumerator Get<T>(ApiSettings settings, string path, Action<T> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get(settings.Url(path));
        return Send(request, settings.TimeoutSec, onSuccess, onError);
    }

    public static IEnumerator PostJson<T>(ApiSettings settings, string path, object body, Action<T> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(body);
        UnityWebRequest request = UnityWebRequest.Post(settings.Url(path), json, "application/json");
        return Send(request, settings.TimeoutSec, onSuccess, onError);
    }

    private static IEnumerator Send<T>(UnityWebRequest request, int timeoutSec, Action<T> onSuccess, Action<string> onError)
    {
        using (request)
        {
            request.timeout = timeoutSec;
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string error = DescribeError(request);
                Debug.LogWarning($"ApiClient: {request.method} {request.url} failed: {error}");
                onError?.Invoke(error);
                yield break;
            }

            T data;
            try
            {
                data = JsonUtility.FromJson<T>(request.downloadHandler.text);
            }
            catch (ArgumentException)
            {
                onError?.Invoke("invalid JSON in response");
                yield break;
            }

            if (data == null)
            {
                onError?.Invoke("empty response");
                yield break;
            }

            onSuccess?.Invoke(data);
        }
    }

    private static string DescribeError(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            return $"HTTP {request.responseCode}";
        }
        return request.error;
    }
}
