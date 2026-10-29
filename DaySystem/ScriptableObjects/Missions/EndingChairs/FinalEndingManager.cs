using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class FinalEndingManager : MonoBehaviour
{
    public static FinalEndingManager Instance { get; private set; }

    [Header("Player & Camera Settings")]
    public GameObject realPlayer;
    public GameObject realPlayerCamera;
    public GameObject decoyPlayer;
    public GameObject decoyCamera;

    [System.Serializable]
    public class FinalEndingNPCSetup
    {
        public NPCID npcId;
        public NPCAnchor boatAnchorNPC;
        public DialogueAnchor dialogueAnchor;
    }

    [System.Serializable]
    public class FinalEndingMapping
    {
        public FinalEndingMissionSO mission;

        [Header("NPCs & Scene Settings")]
        public List<FinalEndingNPCSetup> npcsSetup;
    }

    [Header("Missions Mapping")]
    public List<FinalEndingMapping> mappings;

    private FinalEndingMissionSO _currentMission;
    private readonly Dictionary<NPC, NPCAnchor> _previousNPCAnchors = new Dictionary<NPC, NPCAnchor>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void StartFinalEnding(FinalEndingMissionSO mission)
    {
        FinalEndingMapping mapping = GetMappingForMission(mission);

        if (mapping == null) return;

        _currentMission = mission;
        StartCoroutine(FinalEndingSequenceRoutine(mapping));
    }

    private IEnumerator FinalEndingSequenceRoutine(FinalEndingMapping mapping)
    {
        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(true);

        if (decoyPlayer != null) decoyPlayer.SetActive(true);
        if (decoyCamera != null) decoyCamera.SetActive(true);
        if (realPlayerCamera != null) realPlayerCamera.SetActive(false);

        InitializeParticipatingNPCs(mapping);

        if (mapping.mission != null && mapping.mission.dialogueSO != null && mapping.mission.dialogueSO.sequence != null && mapping.mission.dialogueSO.sequence.Count > 0)
        {
            DialogueUIManager.Instance.StartFinalEndingDialogueSequence(
                mapping.mission.dialogueSO.sequence,
                npcId => GetDialogueAnchor(mapping, npcId)
            );

            yield return new WaitWhile(() => DialogueUIManager.Instance.IsInDialogue);
        }

        EndFinalEndingSequence(mapping);
    }

    private void InitializeParticipatingNPCs(FinalEndingMapping mapping)
    {
        _previousNPCAnchors.Clear();

        if (mapping.npcsSetup == null) return;

        foreach (FinalEndingNPCSetup setup in mapping.npcsSetup)
        {
            NPC npc = GameStateManager.Instance != null ? GameStateManager.Instance.GetNPC(setup.npcId) : null;
            if (npc != null)
            {
                _previousNPCAnchors[npc] = npc.defaultAnchor;
                npc.InitializeNPC(setup.boatAnchorNPC);
            }
        }
    }

    private void EndFinalEndingSequence(FinalEndingMapping mapping)
    {
        foreach (KeyValuePair<NPC, NPCAnchor> pair in _previousNPCAnchors)
        {
            if (pair.Key != null && pair.Value != null) pair.Key.InitializeNPC(pair.Value);
        }

        _previousNPCAnchors.Clear();

        if (decoyCamera != null) decoyCamera.SetActive(false);
        if (decoyPlayer != null) decoyPlayer.SetActive(false);
        if (realPlayerCamera != null) realPlayerCamera.SetActive(true);

        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(false);

        if (_currentMission != null)
        {
            _currentMission.FinishMission();
            _currentMission = null;
        }
    }

    private FinalEndingMapping GetMappingForMission(FinalEndingMissionSO mission)
    {
        if (mappings == null) return null;

        foreach (FinalEndingMapping mapping in mappings)
        {
            if (mapping.mission == mission) return mapping;
        }

        return null;
    }

    private DialogueAnchor GetDialogueAnchor(FinalEndingMapping mapping, NPCID npcId)
    {
        if (mapping == null || mapping.npcsSetup == null) return null;

        foreach (FinalEndingNPCSetup setup in mapping.npcsSetup)
        {
            if (setup.npcId == npcId) return setup.dialogueAnchor;
        }

        return null;
    }
}
