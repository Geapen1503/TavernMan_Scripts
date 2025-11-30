using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CricketsSoundManager : MonoBehaviour
{
    [Header("Cricket Sounds")]
    public List<AudioSource> crickets;

    public List<AudioSource> Crickets { get => crickets; set => crickets = value; }
}
