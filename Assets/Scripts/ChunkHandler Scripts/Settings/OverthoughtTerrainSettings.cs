using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class OverthoughtTerrainSettings : NoiseSettings {
    [Range(0, 1)]
    public float noiseStrength = 0.5f;
    [Min(0)]
    public float heightScale = 20f;

    [Min(float.Epsilon)]
    public float terrainScale = 1;
    [Range(1, 8)]
    public int terrainOctaves = 8;
    [Range(0, 1)]
    public float terrainPersistance = 0.5f;
    [Min(1)]
    public float terrainLacunarity = 2;

    public float floorHeight;
    public float floorStrength;

    public bool useTerracing;
    [Min(0)]
    public float terraceHeight;
}
