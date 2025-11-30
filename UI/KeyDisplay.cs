using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;

public class KeyDisplay : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText;

    public static KeyDisplay Instance { get; private set; }

    protected virtual void Awake()
    {
        pressKeyText.text = string.Empty;

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple KeyDisplay instances detected. Keeping the first one.");
            return;
        }
        Instance = this;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void LateUpdate()
    {
        
    }
}
