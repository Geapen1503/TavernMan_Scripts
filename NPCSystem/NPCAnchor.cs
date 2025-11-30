using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class NPCAnchor : MonoBehaviour
{
    private string anchorId;
    public string defaultAnchorAnimation;

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
