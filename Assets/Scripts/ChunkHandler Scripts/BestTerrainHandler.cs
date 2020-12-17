using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestTerrainHandler : HandlerTemplate {
    [Header("Noise Settings")]
    [Min(float.Epsilon)]
    public float scale = 1;
    [Range(1, 8)]
    public int octaves = 8;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Min(1)]
    public float lacunarity = 2;
    public int seed = 0;
    public float floorHeight = 0;
    [Min(0)]
    public float floorStrength = 0;
    [Min(0)]
    public float amplitude = 0;

    public override void GenerateValues() {
        values = DensityFunction.GenerateBestTerrainValues(terrainSize, gridSize, center, scale, octaves, persistance, lacunarity, seed, amplitude, floorHeight, floorStrength);
    }
}
