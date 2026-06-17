using System.Collections.Generic;
using UnityEngine;

public class ExtraNPCTableSetup : MonoBehaviour
{
    public List<Transform> slots;
    public List<GameObject> cardsToDraw;
    public List<GameObject> coins;

    public Transform foldTransform;

    [HideInInspector] public bool hasFolded;
    [HideInInspector] public int foldAtCardIndex;

    public void InitializeForNewGame()
    {
        hasFolded = false;

        foldAtCardIndex = Random.Range(2, 5);
    }

    public void ActivateCoin(int index)
    {
        if (index >= 0 && index < coins.Count && coins[index] != null)
        {
            coins[index].SetActive(true);
        }
    }

    public void ResetVisuals()
    {
        foreach (var coin in coins)
        {
            if (coin != null) coin.SetActive(false);
        }
    }
}