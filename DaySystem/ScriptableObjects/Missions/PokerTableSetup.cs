using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerTableSetup : MonoBehaviour
{
    [Header("Camera Settings & UI")]
    public GameObject pokerCamera;
    public GameObject pokerCanvas;

    [Header("Game refs")]
    public GameObject cardDeck;
    public List<Transform> playerSlots;
    public List<Transform> npcSlots;

    [Header("Coins")]
    public List<GameObject> playerCoins;
    public List<GameObject> opponentCoins;

    public List<ExtraNPCTableSetup> extraNPCs;

    public void ActivateCoinForRound(int index)
    {
        if (index >= 0 && index < playerCoins.Count && playerCoins[index] != null)
        {
            playerCoins[index].SetActive(true);
        }

        if (index >= 0 && index < opponentCoins.Count && opponentCoins[index] != null)
        {
            opponentCoins[index].SetActive(true);
        }
    }

    public void ResetTableVisuals()
    {
        foreach (var coin in playerCoins) if (coin != null) coin.SetActive(false);
        foreach (var coin in opponentCoins) if (coin != null) coin.SetActive(false);

        foreach (var npc in extraNPCs) if (npc != null) npc.ResetVisuals();

        if (cardDeck != null) cardDeck.SetActive(false);
        if (pokerCanvas != null) pokerCanvas.SetActive(false);
        if (pokerCamera != null) pokerCamera.SetActive(false);
    }
}