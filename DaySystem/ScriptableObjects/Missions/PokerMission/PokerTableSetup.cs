using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerTableSetup : MonoBehaviour
{
    [Header("Camera Settings & UI")]
    public GameObject pokerCamera;
    public GameObject pokerCanvas;

    [Header("Audio Settings")]
    public AudioSource pokerAudioSource;

    [Header("Game refs")]
    public Transform showdownCameraPosition;
    public GameObject cardDeck;

    public List<Transform> playerSlots;
    public List<Transform> npcSlots;

    public List<Transform> centerPlayerSlots;
    public List<Transform> centerOpponentSlots;
    

    [Header("Coins")]
    public List<GameObject> playerCoins;
    public List<GameObject> opponentCoins;

    public List<ExtraNPCTableSetup> extraNPCs;

    private Vector3 pokerCamPosition;
    private Quaternion pokerCamRotation;

    private void Start()
    {
        pokerCamPosition = pokerCamera.transform.position;
        pokerCamRotation = pokerCamera.transform.rotation;
    }

    public void ActivateCoinForRound(int index)
    {
        ActivateCoin(playerCoins, index);
        ActivateCoin(opponentCoins, index);
    }

    public void ResetTableVisuals()
    {
        DisableCoins(playerCoins);
        DisableCoins(opponentCoins);

        foreach (var npc in extraNPCs)
        {
            if (npc != null) npc.ResetVisuals();
        }

        SetActive(cardDeck, false);
        SetActive(pokerCanvas, false);
        SetActive(pokerCamera, false);
        ResetPokerCam(); 
    }

    private static void ActivateCoin(List<GameObject> coins, int index)
    {
        if (index < 0 || index >= coins.Count) return;

        if (coins[index] != null) coins[index].SetActive(true);
    }

    private static void DisableCoins(List<GameObject> coins)
    {
        foreach (var coin in coins)
        {
            if (coin != null) coin.SetActive(false);
        }
    }

    private void ResetPokerCam()
    {
        if (pokerCamPosition != null && pokerCamRotation != null)
        {
            pokerCamera.transform.position = pokerCamPosition;
            pokerCamera.transform.rotation = pokerCamRotation;
        }
    }

    private static void SetActive(GameObject obj, bool value)
    {
        if (obj != null) obj.SetActive(value);
    }

}