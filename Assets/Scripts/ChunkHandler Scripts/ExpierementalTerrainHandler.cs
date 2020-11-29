using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpierementalTerrainHandler : HandlerTemplate {
    [Header("Terrain Settings")]
    [Min(float.Epsilon)]
    public float scale = 1;
    [Range(1, 8)]
    public int octaves = 8;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Min(1)]
    public float lacunarity = 2;
    public int seed = 0;
    public float amplitude = 0;
    public float floorHeight = 0;
    [Min(0)]
    public float floorStrength = 10;
    public bool useTerracing = false;
    [Min(0.0001f)]
    public float terraceHeight = 1f;

    public override void GenerateValues() {
        values = DensityFunction.GenerateExpierementalTerrainValues(terrainSize, gridSize, center, scale, octaves, persistance, lacunarity, seed, amplitude, floorHeight, floorStrength, useTerracing, terraceHeight);
    }
}
