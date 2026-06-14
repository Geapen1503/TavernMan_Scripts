using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class EndDayManager : MonoBehaviour
{
    public static EndDayManager Instance { get; private set; }

    [Header("Player & Camera Settings")]
    public GameObject realPlayer;
    public GameObject realPlayerCamera;
    public GameObject decoyPlayer;
    public GameObject decoyCamera;

    [System.Serializable]
    public class ChairMissionMapping
    {
        public EndDayMissionSO mission;

        [Header("NPC & Scene Settings")]
        public NPCID npcId;
        public NPCAnchor chairAnchorNPC;
        public DialogueAnchor chairDialogueAnchor;
    }

    [Header("Missions Mapping")]
    public List<ChairMissionMapping> mappings;

    private EndDayMissionSO _currentMission;
    private NPC _currentActiveNPC;
    private NPCAnchor _previousNPCAnchor;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void StartChairSequence(EndDayMissionSO mission)
    {
        ChairMissionMapping mapping = GetMappingForMission(mission);

        if (mapping == null)
        {
            Debug.LogError($"[EndDayManager] No conf for {mission.name}. Hard lock.");
            return;
        }

        _currentMission = mission;
        StartCoroutine(ChairSequenceRoutine(mapping));
    }

    private IEnumerator ChairSequenceRoutine(ChairMissionMapping mapping)
    {
        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(true);

        if (decoyPlayer != null) decoyPlayer.SetActive(true);
        if (decoyCamera != null) decoyCamera.SetActive(true);
        if (realPlayerCamera != null) realPlayerCamera.SetActive(false);

        _currentActiveNPC = GameStateManager.Instance.GetNPC(mapping.npcId);

        if (_currentActiveNPC != null)
        {
            _previousNPCAnchor = _currentActiveNPC.defaultAnchor;
            _currentActiveNPC.InitializeNPC(mapping.chairAnchorNPC);
        }

        if (mapping.mission.dialogueSO != null && mapping.chairDialogueAnchor != null)
        {
            DialogueUIManager.Instance.MoveCanvasToNPC(mapping.chairDialogueAnchor);
            DialogueUIManager.Instance.StartEndingDialogueSequence(mapping.mission.dialogueSO.sequence, mapping.npcId);

            yield return new WaitWhile(() => DialogueUIManager.Instance.IsInDialogue);
        }

        EndChairSequence(mapping);
    }

    private void EndChairSequence(ChairMissionMapping mapping)
    {
        if (_currentActiveNPC != null && _previousNPCAnchor != null)
        {
            _currentActiveNPC.InitializeNPC(_previousNPCAnchor);
        }

        if (decoyCamera != null) decoyCamera.SetActive(false);
        if (decoyPlayer != null) decoyPlayer.SetActive(false);
        if (realPlayerCamera != null) realPlayerCamera.SetActive(true);

        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(false);

        if (_currentMission != null)
        {
            _currentMission.FinishMission();
            _currentMission = null;
        }

        _currentActiveNPC = null;
        _previousNPCAnchor = null;
    }

    private ChairMissionMapping GetMappingForMission(EndDayMissionSO mission)
    {
        foreach (ChairMissionMapping mapping in mappings)
        {
            if (mapping.mission == mission) return mapping;
        }
        return null;
    }
}
