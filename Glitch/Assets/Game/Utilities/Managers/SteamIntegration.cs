using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamIntegration : MonoBehaviour
{
    public static SteamIntegration instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(3369700);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
    private void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    public static bool IsThisAchievementUnlocked(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        return ach.State;

    }

    public static void UnlockThisAchievement(string id)
    {
        if (!IsThisAchievementUnlocked(id))
        {
            var ach = new Steamworks.Data.Achievement(id);
            ach.Trigger();
        }
    }

    public static void ClearAchievementStatus(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Clear();
    }

    public static void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }
}
