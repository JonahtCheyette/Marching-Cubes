using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerTemplate : MonoBehaviour {
    public Vector3Int chunkSize = Vector3Int.one * 17;
    public Vector3Int terrainSize;
    [Min(0.0001f)]
    public float gridSize = 1;
    public float surfaceLevel = 0.5f;
    public Vector3 center;
    public Material meshMaterial;
    public bool AutoUpdate = false;
    public bool showMinAndMaxValues = false;
    public bool usePercentageSurfaceLevel = true;

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
        if (usePercentageSurfaceLevel) {
            SetIsoLevel();
            terrainChunkData = ChunkGenerator.Generate(values, terrainSize, chunkSize, center, gridSize, isoLevel);
        } else {
            terrainChunkData = ChunkGenerator.Generate(values, terrainSize, chunkSize, center, gridSize, surfaceLevel);
        }
        ResizeAndCreateChunks();
    }

    private void ResizeAndCreateChunks() {
        //deleting everything if there are unexplained children of the gameobject
        DeleteUnexplainedChildren();

        //deleting any references to chunks that have been deleted in the editor or by other means
        RemoveDeletedChunks();

        //getting rid of any extra chunks
        GetRidOfUnneededChunks();

        //remaking/ creating new chunks
        CreateChunks();
    }

    private void DeleteUnexplainedChildren() {
        if(gameObject.transform.childCount > chunks.Count) {

            for (int i = gameObject.transform.childCount - 1; i >= 0; i--) {
                DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
            }
            chunks.Clear();
        }
    }

    private void RemoveDeletedChunks() {
        for (int i = chunks.Count - 1; i >= 0; i--) {
            if (chunks[i].HasBeenDeleted() || chunks[i] == null) {
                chunks.RemoveAt(i);
            }
        }
    }

    private void GetRidOfUnneededChunks() {
        while (chunks.Count > terrainChunkData.Length) {
            RemoveTerrainChunkFromChunks(chunks.Count - 1);
        }
    }

    private void RemoveTerrainChunkFromChunks(int i) {
        chunks[i].Destroy();
        chunks.RemoveAt(i);
    }

    private void CreateChunks() {
        for (int i = 0; i < terrainChunkData.Length; i++) {
            if (i < chunks.Count) {
                chunks[i].Remake(terrainChunkData[i]);
            } else {
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

        isoLevel = Utility.Interpolate(min, max, surfaceLevel);
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

    private void OnValidate() {
        if (chunkSize.x < 2) {
            chunkSize.x = 2;
        }
        if (chunkSize.y < 2) {
            chunkSize.y = 2;
        }
        if (chunkSize.z < 2) {
            chunkSize.z = 2;
        }

        while (15 * (chunkSize.x - 1) * (chunkSize.y - 1) * (chunkSize.z - 1) > 65535) {
            if (chunkSize.x >= chunkSize.y) {
                if (chunkSize.x >= chunkSize.z) {
                    //x is greatest
                    chunkSize.x--;
                } else {
                    //z is greatest
                    chunkSize.z--;
                }
            } else {
                if (chunkSize.y >= chunkSize.z) {
                    //y is greatest
                    chunkSize.y--;
                } else {
                    //z is greatest
                    chunkSize.z--;
                }
            }
        }

        if (terrainSize.x < 2) {
            terrainSize.x = 2;
        }
        if (terrainSize.y < 2) {
            terrainSize.y = 2;
        }
        if (terrainSize.z < 2) {
            terrainSize.z = 2;
        }

        if (usePercentageSurfaceLevel) {
            if(surfaceLevel < 0) {
                surfaceLevel = 0;
            } else if (surfaceLevel > 1) {
                surfaceLevel = 1;
            }
        }

        //have to do it this way, otherwise console gets spammed with hundreds of warning messages messages, really annoying workaroud considering 
        //this has the same functionality of just having the function in OnValidate
        UnityEditor.EditorApplication.delayCall += UpdateChunksIfAutoUpdateIsOn;
    }

    private void UpdateChunksIfAutoUpdateIsOn() {
        UnityEditor.EditorApplication.delayCall -= UpdateChunksIfAutoUpdateIsOn;
        if (AutoUpdate) {
            GenerateChunks();
        }
    }

    private void OnApplicationQuit() {
        ChunkGenerator.DestroyBuffers();
        DensityFunction.DestroyBuffer();
    }
}
