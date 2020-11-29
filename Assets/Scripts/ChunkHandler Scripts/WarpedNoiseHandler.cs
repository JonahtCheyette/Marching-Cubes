using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpedNoiseHandler : HandlerTemplate {
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

    public WarpSettings[] warpSettings;

    public override void GenerateValues() {
        values = DensityFunction.GenerateWarpedNoiseValues(terrainSize, gridSize, center, scale, octaves, persistance, lacunarity, seed, amplitude, floorHeight, floorStrength, warpSettings);
    }

    public override void OnValidate() {
        base.OnValidate();

        if(warpSettings.Length > 3) {
            WarpSettings[] oldSettings = new WarpSettings[3];
            for (int i = 0; i < 3; i++) {
                oldSettings[i] = warpSettings[i];
            }
            warpSettings = new WarpSettings[3];
            for (int i = 0; i < 3; i++) {
                warpSettings[i] = oldSettings[i];
            }
        }

        if(warpSettings.Length == 0) {
            warpSettings = new WarpSettings[1];
        }
    }
}

[System.Serializable]
public struct WarpSettings {
    [Min(0.0001f)]
    public float scale;
    public Vector3 offsetOne;
    public Vector3 offsetTwo;
    public Vector3 offsetThree;
}
