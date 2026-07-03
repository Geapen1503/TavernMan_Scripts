using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxMissionManager : MonoBehaviour
{
    public static JukeboxMissionManager Instance { get; private set; }

    public List<JukeboxMissionSO> jukeboxMissions;

    public Action OnPlaylistChangedAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void NotifyPlaylistChanged()
    {
        OnPlaylistChangedAction?.Invoke();
    }
}
