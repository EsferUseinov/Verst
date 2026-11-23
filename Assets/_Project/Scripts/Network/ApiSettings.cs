using UnityEngine;

[CreateAssetMenu(menuName = "Verst/Api Settings", fileName = "Api_Local")]
public class ApiSettings : ScriptableObject
{
    [SerializeField] private string baseUrl = "http://localhost:8080";
    [SerializeField, Min(1)] private int timeoutSec = 5;

    public int TimeoutSec => timeoutSec;

    public string Url(string path)
    {
        return baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
    }
}
