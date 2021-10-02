using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BestTerrainSettings : NoiseSettings {
    public float floorHeight = 0;
    [Min(0)]
    public float floorStrength = 0;
    [Min(0)]
    public float amplitude = 0;
}
