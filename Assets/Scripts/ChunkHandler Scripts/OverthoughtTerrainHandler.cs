using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverthoughtTerrainHandler : HandlerTemplate {
    public int seed = 0;

    [Header("Noise Settings")]
    [Min(float.Epsilon)]
    public float scale = 1;
    [Range(1, 8)]
    public int octaves = 8;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Min(1)]
    public float lacunarity = 2;

    [Header("Terrain Settings")]

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

    public override void GenerateValues() {
        values = DensityFunction.GenerateOverthoughtTerrainValues(terrainSize, gridSize, center, scale, octaves, persistance, lacunarity, seed, noiseStrength, heightScale, floorHeight, floorStrength, terrainScale, terrainOctaves, terrainPersistance, terrainLacunarity, useTerracing, terraceHeight);
    }
}
