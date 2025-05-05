using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveV2", menuName = "ScriptableObjects/WaveV2", order = 1)]
public class WaveV2 : ScriptableObject
{
    [field: SerializeField]
    public SOenemy[] EnemyTypes { get; private set; }

    [field: SerializeField]
    public float TimeBeforeThisWave { get; private set; }



    [field: SerializeField]
    public float[] NumberTypesToSpawn { get; private set; }
}