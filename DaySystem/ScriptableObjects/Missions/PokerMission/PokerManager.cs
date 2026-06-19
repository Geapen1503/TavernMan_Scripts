using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

[RequireComponent(typeof(AudioSource))]
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

    private struct CardTransformData
    {
        public GameObject card;
        public Vector3 position;
        public Quaternion rotation;
    }

    [Header("Missions Mapping")]
    public List<PokerMissionMapping> mappings;

    private readonly List<CardTransformData> _recordedCards = new();

    private PokerMissionSO _currentMission;

    private Vector3 _originalCamPos;
    private Quaternion _originalCamRot;

    private PokerAction _currentRoundAction = PokerAction.None;

    private bool _isWaitingForPlayerInput;
    private int _currentCardIndex;

    private static AudioClip _coinClip;
    private static AudioClip _cardClip;
    private static AudioClip _flipClip;
    private static AudioClip _foldClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadAudioClips();
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
        yield return StartPokerTransition(mapping);

        yield return InitializeNPCs();
        yield return DealInitialCards(mapping);
        yield return PlayRounds(mapping);
        yield return ShowdownRoutine(mapping, _currentCardIndex);

        yield return EndPokerTransition();
    }

    private void PrepareGame(PokerMissionMapping mapping)
    {
        DisablePlayer();

        CacheCameraTransform();

        tableSetup.ResetTableVisuals();

        SetActive(tableSetup.pokerCamera, true);
        SetActive(tableSetup.cardDeck, true);

        RecordOriginalCardTransforms(mapping);

        _currentCardIndex = 0;
    }

    private IEnumerator InitializeNPCs()
    {
        foreach (var npc in tableSetup.extraNPCs) npc?.InitializeForNewGame();

        yield return new WaitForSeconds(0.5f);

        tableSetup.ActivateCoinForRound(0);
        PlayPokerSound(_coinClip);
        yield return new WaitForSeconds(0.15f);

        tableSetup.ActivateCoinForRound(1);
        PlayPokerSound(_coinClip);
        yield return new WaitForSeconds(0.15f);

        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc == null) continue;

            npc.ActivateCoin(0);
            PlayPokerSound(_coinClip);
            yield return new WaitForSeconds(0.12f);
            
            npc.ActivateCoin(1);
            PlayPokerSound(_coinClip);
            yield return new WaitForSeconds(0.12f);
        }
    }

    private IEnumerator DealInitialCards(PokerMissionMapping mapping)
    {
        yield return DistributeCardsToActivePlayers(mapping, 0);
        yield return DistributeCardsToActivePlayers(mapping, 1);

        _currentCardIndex = 2;
    }

    private IEnumerator PlayRounds(PokerMissionMapping mapping)
    {
        int maxCards = GetMaxPlayableCards(mapping);

        while (_currentCardIndex < maxCards)
        {
            yield return HandleNPCRound();
            yield return WaitForPlayerDecision();

            if (_currentRoundAction == PokerAction.Fold) yield break;

            yield return ResolvePlayerAction(mapping);
        }
    }

    private IEnumerator HandleNPCRound()
    {
        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc == null || npc.hasFolded) continue;

            yield return new WaitForSeconds(Random.Range(1f, 2.2f));

            if (_currentCardIndex == npc.foldAtCardIndex)
            {
                yield return HandleNPCFold(npc);
            }
            else
            {
                npc.ActivateCoin(_currentCardIndex);
                PlayPokerSound(_coinClip);
            }
        }
    }

    private IEnumerator HandleNPCFold(ExtraNPCTableSetup npc)
    {
        npc.hasFolded = true;

        PlayPokerSound(_foldClip);

        for (int i = 0; i < _currentCardIndex; i++)
        {
            if (i >= npc.cardsToDraw.Count) continue;

            GameObject card = npc.cardsToDraw[i];

            if (card == null) continue;

            StartCoroutine(MoveCardToSlot(card, npc.foldTransform.position, npc.foldTransform.rotation));
        }

        yield return new WaitForSeconds(0.4f);
    }

    private IEnumerator WaitForPlayerDecision()
    {
        SetActive(tableSetup.pokerCanvas, true);

        _currentRoundAction = PokerAction.None;
        _isWaitingForPlayerInput = true;

        yield return new WaitUntil(() => !_isWaitingForPlayerInput);

        SetActive(tableSetup.pokerCanvas, false);
    }

    private IEnumerator ResolvePlayerAction(PokerMissionMapping mapping)
    {
        if (_currentRoundAction != PokerAction.Call) yield break;

        PlayPokerSound(_coinClip);
        yield return new WaitForSeconds(0.18f);

        PlayPokerSound(_coinClip);

        tableSetup.ActivateCoinForRound(_currentCardIndex);
        yield return new WaitForSeconds(0.2f);

        yield return DistributeCardsToActivePlayers(mapping, _currentCardIndex);
        _currentCardIndex++;
    }

    private IEnumerator DistributeCardsToActivePlayers(PokerMissionMapping mapping, int index)
    {
        yield return MovePlayerCard(mapping, index);
        yield return MoveOpponentCard(mapping, index);

        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc == null || npc.hasFolded) continue;

            if (index >= npc.cardsToDraw.Count || index >= npc.slots.Count) continue;

            PlayPokerSound(_cardClip);
            yield return MoveCardToSlot(
                npc.cardsToDraw[index],
                npc.slots[index].position,
                npc.slots[index].rotation);
        }
    }

    private IEnumerator MovePlayerCard(PokerMissionMapping mapping, int index)
    {
        if (index >= mapping.playerCardsToDraw.Count || index >= mapping.playerSlots.Count) yield break;

        PlayPokerSound(_cardClip);
        yield return MoveCardToSlot(
            mapping.playerCardsToDraw[index],
            mapping.playerSlots[index].position,
            mapping.playerSlots[index].rotation);
    }

    private IEnumerator MoveOpponentCard(PokerMissionMapping mapping, int index)
    {
        if (index >= mapping.npcCardsToDraw.Count || index >= mapping.npcSlots.Count) yield break;

        PlayPokerSound(_cardClip);
        yield return MoveCardToSlot(
            mapping.npcCardsToDraw[index],
            mapping.npcSlots[index].position,
            mapping.npcSlots[index].rotation);
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
        _recordedCards.Clear();

        foreach (var card in mapping.playerCardsToDraw) RecordCard(card);
        foreach (var card in mapping.npcCardsToDraw) RecordCard(card);

        foreach (var npc in tableSetup.extraNPCs)
        {
            if (npc == null) continue;

            foreach (var card in npc.cardsToDraw) RecordCard(card);
        }
    }

    private void RecordCard(GameObject card)
    {
        if (card == null) return;

        _recordedCards.Add(new CardTransformData
        {
            card = card,
            position = card.transform.position,
            rotation = card.transform.rotation
        });
    }

    private IEnumerator ShowdownRoutine(PokerMissionMapping mapping, int cardCount)
    {
        PlayPokerSound(_flipClip);

        StartCoroutine(MoveCameraToShowdown());

        for (int i = 0; i < cardCount; i++)
        {
            MoveShowdownPlayerCard(mapping, i);
            MoveShowdownOpponentCard(mapping, i);
        }

        yield return new WaitForSeconds(4f);
    }

    private void MoveShowdownPlayerCard(PokerMissionMapping mapping, int index)
    {
        if (index >= mapping.playerCardsToDraw.Count) return;

        StartCoroutine(
            MoveCardToSlot(
                mapping.playerCardsToDraw[index],
                tableSetup.centerPlayerSlots[index].position,
                tableSetup.centerPlayerSlots[index].rotation));
    }

    private void MoveShowdownOpponentCard(PokerMissionMapping mapping, int index)
    {
        if (index >= mapping.npcCardsToDraw.Count) return;

        StartCoroutine(
            MoveCardToSlot(
                mapping.npcCardsToDraw[index],
                tableSetup.centerOpponentSlots[index].position,
                tableSetup.centerOpponentSlots[index].rotation));
    }

    private IEnumerator MoveCameraToShowdown()
    {
        if (tableSetup.pokerCamera == null || tableSetup.showdownCameraPosition == null) yield break;

        Transform cameraTransform = tableSetup.pokerCamera.transform;

        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        Vector3 targetPos = tableSetup.showdownCameraPosition.position;
        Quaternion targetRot = tableSetup.showdownCameraPosition.rotation;

        float elapsed = 0f;
        float duration = 1.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            cameraTransform.position = Vector3.Lerp(startPos, targetPos, t);
            cameraTransform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        cameraTransform.position = targetPos;
        cameraTransform.rotation = targetRot;
    }

    private void EndPokerSequence()
    {
        RestoreCards();

        tableSetup.ResetTableVisuals();

        EnablePlayer();

        if (_currentMission != null)
        {
            _currentMission.FinishMission();
            _currentMission = null;
        }
    }

    private void RestoreCards()
    {
        foreach (var cardData in _recordedCards)
        {
            if (cardData.card == null) continue;

            cardData.card.transform.position = cardData.position;
            cardData.card.transform.rotation = cardData.rotation;
        }
    }

    private void DisablePlayer()
    {
        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(true);

        SetActive(realPlayerCamera, false);
    }

    private void EnablePlayer()
    {
        SetActive(realPlayerCamera, true);

        if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.TogglePlayerForChairEnding(false);
    }

    private void CacheCameraTransform()
    {
        if (tableSetup.pokerCamera == null) return;

        _originalCamPos = tableSetup.pokerCamera.transform.position;
        _originalCamRot = tableSetup.pokerCamera.transform.rotation;
    }

    private int GetMaxPlayableCards(PokerMissionMapping mapping)
    {
        return Mathf.Min(5, Mathf.Max(mapping.playerCardsToDraw.Count, mapping.npcCardsToDraw.Count));
    }

    private IEnumerator StartPokerTransition(PokerMissionMapping mapping)
    {
        PlayerUI playerUI = PlayerUI.Instance;

        if (playerUI != null) playerUI.ShowCinematicOverlay();
        yield return new WaitForSeconds(0.5f);

        PrepareGame(mapping);

        yield return new WaitForFixedUpdate();
        yield return null;

        if (playerUI != null)
        {
            playerUI.HidePressKey();
            playerUI.HideCinematicOverlay();
        }
    }

    private IEnumerator EndPokerTransition()
    {
        PlayerUI playerUI = PlayerUI.Instance;

        if (playerUI != null) playerUI.ShowCinematicOverlay();
        yield return new WaitForSeconds(0.5f);

        EndPokerSequence();

        yield return null;
        if (playerUI != null) playerUI.HideCinematicOverlay();
    }

    private void LoadAudioClips()
    {
        if (_coinClip == null) _coinClip = Resources.Load<AudioClip>("Audio/Tavern/Poker/drop-coin");
        if (_cardClip == null) _cardClip = Resources.Load<AudioClip>("Audio/Tavern/Poker/giving-card");
        if (_flipClip == null) _flipClip = Resources.Load<AudioClip>("Audio/Tavern/Poker/flipcards");
        if (_foldClip == null) _foldClip = Resources.Load<AudioClip>("Audio/Tavern/Poker/fold-cards");
    }

    private void PlayPokerSound(AudioClip clip)
    {
        if (tableSetup != null && tableSetup.pokerAudioSource != null && clip != null)
        {
            tableSetup.pokerAudioSource.PlayOneShot(clip);
        }
    }

    private PokerMissionMapping GetMappingForMission(PokerMissionSO mission)
    {
        foreach (var mapping in mappings) if (mapping.mission == mission) return mapping;

        return null;
    }

    private static void SetActive(GameObject obj, bool value)
    {
        if (obj != null) obj.SetActive(value);
    }
}