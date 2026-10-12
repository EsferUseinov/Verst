using System;
using UnityEngine;

public class ProfileStore : MonoBehaviour
{
    private const string PrefsKey = "Verst.OperatorProfile";

    public event Action<OperatorProfile> Changed;

    private OperatorProfile current;

    public OperatorProfile Current => current ??= Load();

    public void Save(OperatorProfile profile)
    {
        current = profile.Clone();
        PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(current));
        PlayerPrefs.Save();
        Changed?.Invoke(current);
    }

    private static OperatorProfile Load()
    {
        string json = PlayerPrefs.GetString(PrefsKey, "");
        if (string.IsNullOrEmpty(json))
        {
            return new OperatorProfile();
        }

        try
        {
            return JsonUtility.FromJson<OperatorProfile>(json) ?? new OperatorProfile();
        }
        catch (ArgumentException)
        {
            Debug.LogWarning("ProfileStore: saved profile is corrupted, using defaults");
            return new OperatorProfile();
        }
    }
}
