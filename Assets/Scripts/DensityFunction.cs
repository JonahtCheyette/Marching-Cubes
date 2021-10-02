using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DensityFunction {
    private static ComputeShader noiseShader = (ComputeShader)Resources.Load("ComputeShaders/NoiseDensity");
    private static ComputeShader overthoughtTerrainShader = (ComputeShader)Resources.Load("ComputeShaders/OverthoughtTerrain");
    private static ComputeShader expierementalTerrainShader = (ComputeShader)Resources.Load("ComputeShaders/ExpierementalTerrain");
    private static ComputeShader warpedNoiseShader = (ComputeShader)Resources.Load("ComputeShaders/WarpedNoise");
    private static ComputeShader sphericalNoiseShader = (ComputeShader)Resources.Load("ComputeShaders/SphericalNoise");
    private static ComputeShader bestTerrainShader = (ComputeShader)Resources.Load("ComputeShaders/BestTerrain");
    private static ComputeBuffer points;
    private static Vector4[] values;

    public static Vector4[] GenerateNoiseValues(MarchingCubeSettings marchingCubeSettings, Vector3 center, PureNoiseSettings settings) {
        SetupGeneration(noiseShader, marchingCubeSettings.size, marchingCubeSettings.gridSize, center);

        noiseShader.SetFloat("scale", settings.scale);

        Vector4[] octaveOffsets = new Vector4[settings.octaves];

        System.Random rand = new System.Random(settings.seed);

        for (int i = 0; i < settings.octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(noiseShader, settings.octaves, settings.persistance, settings.lacunarity, octaveOffsets);

        DispatchShader(noiseShader, marchingCubeSettings.size);

        if (!Application.isPlaying) {
            DestroyBuffer();
        }

        return values;
    }
    
    public static Vector4[] GenerateOverthoughtTerrainValues(MarchingCubeSettings marchingCubeSettings, Vector3 center, OverthoughtTerrainSettings terrainSettings) {
        SetupGeneration(overthoughtTerrainShader, marchingCubeSettings.size, marchingCubeSettings.gridSize, center);

        overthoughtTerrainShader.SetFloat("scale", terrainSettings.scale);
        overthoughtTerrainShader.SetFloat("tScale", terrainSettings.terrainScale);
        overthoughtTerrainShader.SetInt("tOctaves", terrainSettings.terrainOctaves);
        overthoughtTerrainShader.SetFloat("tPersistance", terrainSettings.terrainPersistance);
        overthoughtTerrainShader.SetFloat("tLacunarity", terrainSettings.terrainLacunarity);
        overthoughtTerrainShader.SetFloat("noiseStrength", terrainSettings.noiseStrength);
        overthoughtTerrainShader.SetFloat("heightScale", terrainSettings.heightScale);
        overthoughtTerrainShader.SetFloat("floorHeight", terrainSettings.floorHeight);
        overthoughtTerrainShader.SetFloat("floorStrength", terrainSettings.floorStrength);
        overthoughtTerrainShader.SetBool("useTerracing", terrainSettings.useTerracing);
        overthoughtTerrainShader.SetFloat("terraceHeight", terrainSettings.terraceHeight);

        System.Random rand = new System.Random(terrainSettings.seed);

        Vector4[] octaveOffsets = new Vector4[terrainSettings.octaves];

        for (int i = 0; i < terrainSettings.octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(overthoughtTerrainShader, terrainSettings.octaves, terrainSettings.persistance, terrainSettings.lacunarity, octaveOffsets);

        Vector4[] terrainOffsets = new Vector4[terrainSettings.octaves];

        for (int i = 0; i < terrainSettings.octaves; i++) {
            terrainOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            terrainOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        overthoughtTerrainShader.SetVectorArray("terrainOffsets", terrainOffsets);

        DispatchShader(overthoughtTerrainShader, marchingCubeSettings.size);

        if (!Application.isPlaying) {
            DestroyBuffer();
        }

        return values;
    }

    public static Vector4[] GenerateExpierementalTerrainValues(MarchingCubeSettings marchingCubeSettings, Vector3 center, ExpierementalTerrainSettings terrainSettings) {
        SetupGeneration(expierementalTerrainShader, marchingCubeSettings.size, marchingCubeSettings.gridSize, center);

        expierementalTerrainShader.SetFloat("scale", terrainSettings.scale);
        expierementalTerrainShader.SetFloat("baseAmplitude", terrainSettings.amplitude);
        expierementalTerrainShader.SetFloat("floorHeight", terrainSettings.floorHeight);
        expierementalTerrainShader.SetFloat("floorStrength", terrainSettings.floorStrength);
        expierementalTerrainShader.SetBool("useTerracing", terrainSettings.useTerracing);
        expierementalTerrainShader.SetFloat("terraceHeight", terrainSettings.terraceHeight);

        Vector4[] octaveOffsets = new Vector4[terrainSettings.octaves];

        System.Random rand = new System.Random(terrainSettings.seed);

        for (int i = 0; i < terrainSettings.octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(expierementalTerrainShader, terrainSettings.octaves, terrainSettings.persistance, terrainSettings.lacunarity, octaveOffsets);

        DispatchShader(expierementalTerrainShader, marchingCubeSettings.size);

        if (!Application.isPlaying) {
            DestroyBuffer();
        }

        return values;
    }

    public static Vector4[] GenerateWarpedNoiseValues(MarchingCubeSettings marchingCubeSettings, Vector3 center, WarpedNoiseSettings settings) {
        SetupGeneration(warpedNoiseShader, marchingCubeSettings.size, marchingCubeSettings.gridSize, center);

        warpedNoiseShader.SetFloat("scale", settings.scale);
        warpedNoiseShader.SetFloat("baseAmplitude", settings.amplitude);
        warpedNoiseShader.SetFloat("floorHeight", settings.floorHeight);
        warpedNoiseShader.SetFloat("floorStrength", settings.floorStrength);
        warpedNoiseShader.SetInt("numWarps", Mathf.Min(3, settings.warpSettings.Length));
        List<float> formattedScales = new List<float>();
        float[] scales = settings.warpSettings.Select(x => x.scale).ToArray();
        for(int i = 0; i < 3; i++) {
            if(i < scales.Length) {
                formattedScales.Add(scales[i]);
            } else {
                formattedScales.Add(0);
            }
            //for whatever reason, when assigning to a float array, Setfloats() only reads every 4th value. STUPID.
            for (int j = 0; j < 3; j++) {
                formattedScales.Add(0);
            }
        }
        warpedNoiseShader.SetFloats("warpScales", formattedScales.ToArray());
        warpedNoiseShader.SetVectorArray("warpOffsets1", settings.warpSettings.Select(x => Utility.Vector4FromVector3andValue(x.offsetOne, 0)).ToArray());
        warpedNoiseShader.SetVectorArray("warpOffsets2", settings.warpSettings.Select(x => Utility.Vector4FromVector3andValue(x.offsetTwo, 0)).ToArray());
        warpedNoiseShader.SetVectorArray("warpOffsets3", settings.warpSettings.Select(x => Utility.Vector4FromVector3andValue(x.offsetThree, 0)).ToArray());

        Vector4[] octaveOffsets = new Vector4[settings.octaves];

        System.Random rand = new System.Random(settings.seed);

        for (int i = 0; i < settings.octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(warpedNoiseShader, settings.octaves, settings.persistance, settings.lacunarity, octaveOffsets);

        DispatchShader(warpedNoiseShader, marchingCubeSettings.size);

        if (!Application.isPlaying) {
            DestroyBuffer();
        }

        return values;
    }

    public static Vector4[] GenerateSphericalNoiseValues(MarchingCubeSettings marchingCubeSettings, Vector3 center, SphericalNoiseSettings settings) {
        SetupGeneration(sphericalNoiseShader, marchingCubeSettings.size, marchingCubeSettings.gridSize, center);

        sphericalNoiseShader.SetFloat("scale", settings.scale);
        sphericalNoiseShader.SetFloat("baseAmplitude", settings.amplitude);

        Vector4[] octaveOffsets = new Vector4[settings.octaves];

        System.Random rand = new System.Random(settings.seed);

        for (int i = 0; i < settings.octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(sphericalNoiseShader, settings.octaves, settings.persistance, settings.lacunarity, octaveOffsets);

        DispatchShader(sphericalNoiseShader, marchingCubeSettings.size);

        if (!Application.isPlaying) {
            DestroyBuffer();
        }

        return values;
    }

    public static Vector4[] GenerateBestTerrainValues(MarchingCubeSettings marchingCubeSettings, Vector3 center, BestTerrainSettings terrainSettings) {
        SetupGeneration(bestTerrainShader, marchingCubeSettings.size, marchingCubeSettings.gridSize, center);

        bestTerrainShader.SetFloat("scale", terrainSettings.scale);
        bestTerrainShader.SetFloat("baseAmplitude", terrainSettings.amplitude);
        bestTerrainShader.SetFloat("floorHeight", terrainSettings.floorHeight);
        bestTerrainShader.SetFloat("floorStrength", terrainSettings.floorStrength);

        Vector4[] octaveOffsets = new Vector4[terrainSettings.octaves];

        System.Random rand = new System.Random(terrainSettings.seed);

        for (int i = 0; i < terrainSettings.octaves; i++) {
            octaveOffsets[i].x = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].y = (float)rand.NextDouble() * 200000 - 100000;
            octaveOffsets[i].z = (float)rand.NextDouble() * 200000 - 100000;
        }

        SetPerlinNoiseValues(bestTerrainShader, terrainSettings.octaves, terrainSettings.persistance, terrainSettings.lacunarity, octaveOffsets);

        DispatchShader(bestTerrainShader, marchingCubeSettings.size);

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
