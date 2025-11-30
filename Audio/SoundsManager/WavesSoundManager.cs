using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesSoundManager : MonoBehaviour
{
    [Header("Waves Sounds")]
    public List<AudioSource> waves;

    public List<AudioSource> Waves { get => waves; set => waves = value; }
}
