using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerTemplate : MonoBehaviour {
    public Vector3 center;
    public Material meshMaterial;
    public bool AutoUpdate = false;
    public bool showMinAndMaxValues = false;
    public bool useFlatShading = true;
    public MarchingCubeSettings marchingCubesSettings;

    private List<TerrainChunk> chunks = new List<TerrainChunk>();
    private ChunkData[] terrainChunkData;

    //the surface level adjusted for the min/max of the values
    private float isoLevel;

    [HideInInspector]
    public Vector4[] values;

    public virtual void GenerateValues() {
        print("GenerateValues method not provided in " + GetType().Name);
    }

    public virtual void GenerateChunks() {
        GenerateValues();

        if (showMinAndMaxValues) {
            PrintMinAndMaxValues();
        }

        if (marchingCubesSettings.usePercentageSurfaceLevel) {
            SetIsoLevel();
            terrainChunkData = ChunkGenerator.Generate(values, marchingCubesSettings.size, isoLevel, useFlatShading);
        } else {
            terrainChunkData = ChunkGenerator.Generate(values, marchingCubesSettings.size, marchingCubesSettings.surfaceLevel, useFlatShading);
        }

        CreateChunks();
    }

    private void CreateChunks() {
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
        }
        chunks.Clear();

        for (int i = 0; i < terrainChunkData.Length; i++) {
            if (terrainChunkData[i].vertices.Length > 0) {
                chunks.Add(new TerrainChunk(terrainChunkData[i], gameObject, meshMaterial));
            }
        }
    }

    private void SetIsoLevel() {
        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < values.Length; i++) {
            if (values[i].w < min) {
                min = values[i].w;
            }
            if (values[i].w > max) {
                max = values[i].w;
            }
        }

        isoLevel = Utility.Interpolate(min, max, marchingCubesSettings.surfaceLevel);
    }

    private void PrintMinAndMaxValues() {
        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < values.Length; i++) {
            if (values[i].w < min) {
                min = values[i].w;
            }
            if (values[i].w > max) {
                max = values[i].w;
            }
        }

        print("Min: " + min);
        print("Max: " + max);
    }

    public virtual void OnValidate() {
        if (marchingCubesSettings != null) {
            marchingCubesSettings.OnValuesUpdated -= OnValidate;
            marchingCubesSettings.OnValuesUpdated += OnValidate;
        }

        //have to do it this way, otherwise console gets spammed with hundreds of warning messages messages, really annoying workaroud considering 
        //this has the same functionality of just having the function in OnValidate
        //it just gets called after everything else finishes updating instead
        UnityEditor.EditorApplication.delayCall += UpdateChunksIfAutoUpdateIsOn;
    }

    protected virtual void UpdateChunksIfAutoUpdateIsOn() {
        UnityEditor.EditorApplication.delayCall -= UpdateChunksIfAutoUpdateIsOn;
        if (marchingCubesSettings != null) {
            if (AutoUpdate) {
                GenerateChunks();
            }
        }
    }

    private void OnApplicationQuit() {
        ChunkGenerator.DestroyBuffers();
        DensityFunction.DestroyBuffer();
    }
}
