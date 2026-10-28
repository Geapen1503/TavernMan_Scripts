using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalEndingManager : MonoBehaviour
{
    public static FinalEndingManager Instance { get; private set; }

    [Header("Player & Camera Settings")]
    public GameObject realPlayer;
    public GameObject realPlayerCamera;
    public GameObject decoyPlayer;
    public GameObject decoyCamera;



    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
}
