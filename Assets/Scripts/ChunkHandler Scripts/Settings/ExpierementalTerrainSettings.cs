using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ExpierementalTerrainSettings : NoiseSettings {
    public float amplitude = 0;
    public float floorHeight = 0;
    [Min(0)]
    public float floorStrength = 10;
    public bool useTerracing = false;
    [Min(0.0001f)]
    public float terraceHeight = 1f;
}
