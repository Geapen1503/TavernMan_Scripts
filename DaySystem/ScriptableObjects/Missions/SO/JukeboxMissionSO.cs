using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Jukebox Playlist Change")]
public class JukeboxMissionSO : MissionSO
{
    private bool _playlistChangedDetected = false;

    protected override void MissionContentPlaying()
    {
        _playlistChangedDetected = false;

        if (JukeboxMissionManager.Instance == null) return;

        JukeboxMissionManager.Instance.OnPlaylistChangedAction += HandlePlaylistChanged;

        JukeboxMissionManager.Instance.StartCoroutine(WatchPlaylistChangeRoutine());
    }

    private void HandlePlaylistChanged()
    {
        _playlistChangedDetected = true;
    }

    private IEnumerator WatchPlaylistChangeRoutine()
    {
        while (!_playlistChangedDetected) yield return new WaitForSeconds(0.1f);

        JukeboxMissionManager.Instance.OnPlaylistChangedAction -= HandlePlaylistChanged;

        CompleteMission();
    }
}
