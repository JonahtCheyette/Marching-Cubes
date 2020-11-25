﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DensityFunction {
    private static ComputeShader noiseShader = (ComputeShader)Resources.Load("ComputeShaders/NoiseDensity");
    private static ComputeShader overthoughtTerrainShader = (ComputeShader)Resources.Load("ComputeShaders/OverthoughtTerrain");
    private static ComputeBuffer points;
    private static Vector4[] values;

    public static Vector4[] GenerateNoiseValues(Vector3Int size, float gridSize, Vector3 center, float scale, int octaves, float persistance, float lacunarity, int seed) {
        SetupGeneration(noiseShader, size, gridSize, center);

        noiseShader.SetFloat("scale", scale);

        Vector4[] octaveOffsets = new Vector4[octaves];

        System.Random rand = new System.Random(seed);

        for (int i = 0; i < octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(noiseShader, octaves, persistance, lacunarity, octaveOffsets);

        DispatchShader(noiseShader, size);

        if (!Application.isPlaying) {
            DestroyBuffer();
        }

        return values;
    }
    
    public static Vector4[] GenerateOverthoughtTerrainValues(Vector3Int size, float gridSize, Vector3 center, float scale, int octaves, float persistance, float lacunarity, int seed, float noiseStrength, float heightScale, float floorHeight, float floorStrength, float terrainScale, int terrainOctaves, float terrainPersistance, float terrainLacunarity, bool useTerracing, float terraceHeight) {
        SetupGeneration(overthoughtTerrainShader, size, gridSize, center);

        overthoughtTerrainShader.SetFloat("scale", scale);
        overthoughtTerrainShader.SetFloat("tScale", terrainScale);
        overthoughtTerrainShader.SetInt("tOctaves", terrainOctaves);
        overthoughtTerrainShader.SetFloat("tPersistance", terrainPersistance);
        overthoughtTerrainShader.SetFloat("tLacunarity", terrainLacunarity);
        overthoughtTerrainShader.SetFloat("noiseStrength", noiseStrength);
        overthoughtTerrainShader.SetFloat("heightScale", heightScale);
        overthoughtTerrainShader.SetFloat("floorHeight", floorHeight);
        overthoughtTerrainShader.SetFloat("floorStrength", floorStrength);
        overthoughtTerrainShader.SetBool("useTerracing", useTerracing);
        overthoughtTerrainShader.SetFloat("terraceHeight", terraceHeight);

        System.Random rand = new System.Random(seed);

        Vector4[] octaveOffsets = new Vector4[octaves];

        for (int i = 0; i < octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(overthoughtTerrainShader, octaves, persistance, lacunarity, octaveOffsets);

        Vector4[] terrainOffsets = new Vector4[octaves];

        for (int i = 0; i < octaves; i++) {
            terrainOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            terrainOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        overthoughtTerrainShader.SetVectorArray("terrainOffsets", terrainOffsets);

        DispatchShader(overthoughtTerrainShader, size);

        if (!Application.isPlaying) {
            DestroyBuffer();
        }

        return values;
    }

    private static void SetupGeneration(ComputeShader shader, Vector3Int size, float gridSize, Vector3 center) {
        CreateBuffer(size.x * size.y * size.z);

        values = new Vector4[size.x * size.y * size.z];

        SetStandardShaderValues(shader, size, gridSize, center);
    }

    private static void SetStandardShaderValues(ComputeShader shader, Vector3Int size, float gridSize, Vector3 center) {
        shader.SetInt("sizeX", size.x);
        shader.SetInt("sizeY", size.y);
        shader.SetInt("sizeZ", size.z);
        shader.SetVector("center", center);
        shader.SetFloat("gridSize", gridSize);
    }

    private static void SetPerlinNoiseValues(ComputeShader shader, int octaves, float persistance, float lacunarity, Vector4[] octaveOffsets) {
        shader.SetInt("octaves", octaves);
        shader.SetFloat("persistance", persistance);
        shader.SetFloat("lacunarity", lacunarity);
        shader.SetVectorArray("octaveOffsets", octaveOffsets);
    }

    private static void DispatchShader(ComputeShader shader, Vector3Int size) {
        int kernel = shader.FindKernel("Generate");
        shader.SetBuffer(kernel, "points", points);
        shader.Dispatch(kernel, Mathf.CeilToInt(size.x / 4f), Mathf.CeilToInt(size.y / 4f), Mathf.CeilToInt(size.z / 2f));

        //getting the data from the compute shader
        points.GetData(values);
    }

    private static void CreateBuffer(int numPoints) {
        if (points == null || !points.IsValid() || points.count != numPoints) {
            points = new ComputeBuffer(numPoints, 16);
        }
    }

    public static void DestroyBuffer() {
        if (points != null) {
            points.Release();
        }
    }
}