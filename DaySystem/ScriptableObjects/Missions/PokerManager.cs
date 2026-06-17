using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PokerManager : MonoBehaviour
{
    public static PokerManager Instance { get; private set; }

    [Header("Player & Camera Settings")]
    public GameObject realPlayer;
    public GameObject realPlayerCamera;

    public PokerTableSetup tableSetup;

    [System.Serializable]
    public class PokerMissionMapping
    {
        public PokerMissionSO mission;

        public List<GameObject> playerCardsToDraw;
        public List<GameObject> npcCardsToDraw;

        public List<Transform> playerSlots;
        public List<Transform> npcSlots;
    }

    [Header("Missions Mapping")]
    public List<PokerMissionMapping> mappings;

    private PokerMissionSO _currentMission;

    private List<GameObject> _movedCards = new List<GameObject>();
    private List<Vector3> _movedCardsOriginalPositions = new List<Vector3>();
    private List<Quaternion> _movedCardsOriginalRotations = new List<Quaternion>();

    private enum PokerAction { None, Call, Fold }
    private PokerAction _currentRoundAction = PokerAction.None;
    private bool _isWaitingForPlayerInput = false;
    private int _currentCardIndex = 0;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void StartPokerSequence(PokerMissionSO mission)
    {
        PokerMissionMapping mapping = GetMappingForMission(mission);

        if (mapping == null)
        {
            Debug.LogError($"[PokerManager] No conf for {mission.name}. Hard lock.");
            return;
        }

        _currentMission = mission;
        StartCoroutine(PokerSequenceRoutine(mapping));
    }

    private IEnumerator PokerSequenceRoutine(PokerMissionMapping mapping)
    {
        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(true);
        if (realPlayerCamera != null) realPlayerCamera.SetActive(false);

        tableSetup.ResetTableVisuals();
        if (tableSetup.pokerCamera != null) tableSetup.pokerCamera.SetActive(true);
        if (tableSetup.cardDeck != null) tableSetup.cardDeck.SetActive(true);

        RecordOriginalCardTransforms(mapping);
        _currentCardIndex = 0;

        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc != null) npc.InitializeForNewGame();
        }

        yield return new WaitForSeconds(0.5f);

        tableSetup.ActivateCoinForRound(0);
        tableSetup.ActivateCoinForRound(1);
        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc != null)
            {
                npc.ActivateCoin(0);
                npc.ActivateCoin(1);
            }
        }

        yield return StartCoroutine(DistributeCardsToActivePlayers(mapping, 0));
        yield return StartCoroutine(DistributeCardsToActivePlayers(mapping, 1));

        _currentCardIndex = 2;
        int maxPossibleCards = Mathf.Min(5, Mathf.Max(mapping.playerCardsToDraw.Count, mapping.npcCardsToDraw.Count));

        while (_currentCardIndex < maxPossibleCards)
        {
            foreach (var npc in tableSetup.extraNPCs)
            {
                if (npc == null || npc.hasFolded) continue;

                yield return new WaitForSeconds(Random.Range(1f, 2.2f));

                if (_currentCardIndex == npc.foldAtCardIndex)
                {
                    npc.hasFolded = true;

                    for (int c = 0; c < _currentCardIndex; c++)
                    {
                        if (c < npc.cardsToDraw.Count && npc.cardsToDraw[c] != null)
                        {
                            StartCoroutine(MoveCardToSlot(npc.cardsToDraw[c], npc.foldTransform.position, npc.foldTransform.rotation));
                        }
                    }
                    yield return new WaitForSeconds(0.4f); 
                }
                else
                {
                    npc.ActivateCoin(_currentCardIndex);
                }
            }


            if (tableSetup.pokerCanvas != null) tableSetup.pokerCanvas.SetActive(true);

            _isWaitingForPlayerInput = true;
            _currentRoundAction = PokerAction.None;

            yield return new WaitUntil(() => !_isWaitingForPlayerInput);

            if (tableSetup.pokerCanvas != null) tableSetup.pokerCanvas.SetActive(false);

            if (_currentRoundAction == PokerAction.Fold) break;

            if (_currentRoundAction == PokerAction.Call)
            {
                tableSetup.ActivateCoinForRound(_currentCardIndex);
                yield return StartCoroutine(DistributeCardsToActivePlayers(mapping, _currentCardIndex));
                _currentCardIndex++;
            }
        }

        yield return new WaitForSeconds(1.5f);

        EndPokerSequence(mapping);
    }

    private IEnumerator DistributeCardsToActivePlayers(PokerMissionMapping mapping, int index)
    {
        if (index < mapping.playerCardsToDraw.Count && index < mapping.playerSlots.Count)
        {
            yield return StartCoroutine(MoveCardToSlot(mapping.playerCardsToDraw[index], mapping.playerSlots[index].position, mapping.playerSlots[index].rotation));
        }

        if (index < mapping.npcCardsToDraw.Count && index < mapping.npcSlots.Count)
        {
            yield return StartCoroutine(MoveCardToSlot(mapping.npcCardsToDraw[index], mapping.npcSlots[index].position, mapping.npcSlots[index].rotation));
        }

        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc == null || npc.hasFolded) continue;

            if (index < npc.cardsToDraw.Count && index < npc.slots.Count)
            {
                yield return StartCoroutine(MoveCardToSlot(npc.cardsToDraw[index], npc.slots[index].position, npc.slots[index].rotation));
            }
        }
    }

    private IEnumerator MoveCardToSlot(GameObject card, Vector3 targetPos, Quaternion targetRot)
    {
        float elapsed = 0f;
        float duration = 0.4f;
        Vector3 startPos = card.transform.position;
        Quaternion startRot = card.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = Mathf.Sin(t * Mathf.PI) * 0.15f;

            card.transform.position = Vector3.Lerp(startPos, targetPos, t) + Vector3.up * height;
            card.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        card.transform.position = targetPos;
        card.transform.rotation = targetRot;
    }

    public void OnCallButtonPressed()
    {
        if (!_isWaitingForPlayerInput) return;
        _currentRoundAction = PokerAction.Call;
        _isWaitingForPlayerInput = false;
    }

    public void OnFoldButtonPressed()
    {
        if (!_isWaitingForPlayerInput) return;
        _currentRoundAction = PokerAction.Fold;
        _isWaitingForPlayerInput = false;
    }

    private void RecordOriginalCardTransforms(PokerMissionMapping mapping)
    {
        _movedCards.Clear();
        _movedCardsOriginalPositions.Clear();
        _movedCardsOriginalRotations.Clear();

        foreach (var card in mapping.playerCardsToDraw) { RecordCard(card); }
        foreach (var card in mapping.npcCardsToDraw) { RecordCard(card); }

        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc != null)
            {
                foreach (var card in npc.cardsToDraw) { RecordCard(card); }
            }
        }
    }

    private void RecordCard(GameObject card)
    {
        if (card == null) return;
        _movedCards.Add(card);
        _movedCardsOriginalPositions.Add(card.transform.position);
        _movedCardsOriginalRotations.Add(card.transform.rotation);
    }

    private void EndPokerSequence(PokerMissionMapping mapping)
    {
        for (int i = 0; i < _movedCards.Count; i++)
        {
            if (_movedCards[i] != null)
            {
                _movedCards[i].transform.position = _movedCardsOriginalPositions[i];
                _movedCards[i].transform.rotation = _movedCardsOriginalRotations[i];
            }
        }

        if (tableSetup != null) tableSetup.ResetTableVisuals();

        if (realPlayerCamera != null) realPlayerCamera.SetActive(true);
        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(false);

        if (_currentMission != null)
        {
            _currentMission.FinishMission();
            _currentMission = null;
        }
    }

    private PokerMissionMapping GetMappingForMission(PokerMissionSO mission)
    {
        foreach (PokerMissionMapping mapping in mappings)
        {
            if (mapping.mission == mission) return mapping;
        }
        return null;
    }
}