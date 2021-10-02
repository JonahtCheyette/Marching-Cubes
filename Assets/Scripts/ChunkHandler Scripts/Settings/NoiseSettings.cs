using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSettings : ScriptableObject {
    public int seed = 0;
    [Min(float.Epsilon)]
    public float scale = 1;
    [Range(1, 8)]
    public int octaves = 8;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Min(1)]
    public float lacunarity = 2;

    public event System.Action OnValuesUpdated;

    protected virtual void OnValidate() {
        if (OnValuesUpdated != null) {
            OnValuesUpdated();
        }
    }
}
