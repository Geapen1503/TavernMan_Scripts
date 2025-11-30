using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernSoundManager : MonoBehaviour
{
    [Header("Tavern Sounds")]
    public List<AudioSource> sounds;

    public List<AudioSource> Sounds { get => sounds; set => sounds = value; }
}
