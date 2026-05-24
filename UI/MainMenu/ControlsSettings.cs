using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ControlsSettings
{
    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    public KeyCode jump = KeyCode.Space;
    public KeyCode grab = KeyCode.G;
    public KeyCode talk = KeyCode.T;
    public KeyCode throwKey = KeyCode.F;
    public KeyCode narrator = KeyCode.H;
    public KeyCode pause = KeyCode.Escape;
}