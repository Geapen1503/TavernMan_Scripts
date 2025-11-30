using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class PlayerDialAnchor : MonoBehaviour
{
    private string anchorId;

    void OnValidate()
    {
        if (string.IsNullOrEmpty(anchorId))
        {
            anchorId = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
