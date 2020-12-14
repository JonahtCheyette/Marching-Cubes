using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkGenerator {
    private static ComputeShader polygonizer = (ComputeShader)Resources.Load("ComputeShaders/Polygonize");

    private static ComputeBuffer points;
    private static ComputeBuffer triangleBuffer;
    // Buffer to store count in.
    private static ComputeBuffer countBuffer;

    private static Triangle[] triangles;

    public static ChunkData[] Generate(Vector4[] values, Vector3Int terrainSize, float isoLevel, bool deleteBuffers = false) {
        SetShaderValues(terrainSize, isoLevel);

        GenerateTriangles(values, terrainSize, deleteBuffers);

        return CreateTerrainChunks();
    }

    private static void GenerateTriangles(Vector4[] values, Vector3Int terrainSize, bool deleteBuffers) {
        CreateBuffers(values.Length, terrainSize);

        SetBufferValues(values);

        RunShader(terrainSize);

        if (!Application.isPlaying || deleteBuffers) {
            DestroyBuffers();
        }
    }

    //creates terrainchunks to make the terrain
    private static ChunkData[] CreateTerrainChunks() {
        List<ChunkData> chunks = new List<ChunkData>();

        //the number of triangles, divided by the number of triangles in one mesh
        int numChunks = Mathf.CeilToInt(triangles.Length / 21845f);

        int index = 0;
        for (int i = 0; i < numChunks; i++) {
            List<Vector3> chunkVertices = new List<Vector3>();
            List<int> chunkTriangles = new List<int>();
            for (int j = index; j < index + 21845; j++) {
                if (j < triangles.Length) {
                    for (int k = 0; k < 3; k++) {
                        chunkVertices.Add(triangles[j][k]);
                        chunkTriangles.Add(chunkVertices.Count - 1);
                        //this piece of code is so inefficient, but it gets rid of duplicates. Room for improvement! maybe classify vertices by edge in the polygonizer.
                        //plus, it leaves seams in the terrain due to lighting errors
                        /*
                        if (chunkVertices.Contains(triangles[j][k])) {
                            chunkTriangles.Add(chunkVertices.IndexOf(triangles[j][k]));
                        } else {
                            chunkVertices.Add(triangles[j][k]);
                            chunkTriangles.Add(chunkVertices.Count - 1);
                        }*/
                    }
                } else {
                    break;
                }
            }

            index += 21845;
            chunks.Add(new ChunkData(chunkVertices.ToArray(), chunkTriangles.ToArray()));
        }

        return chunks.ToArray();
    }

    private static void CreateBuffers(int numPoints, Vector3Int terrainSize) {
        if (points == null || !points.IsValid() || points.count != numPoints) {
            points = new ComputeBuffer(numPoints, 16);
        }

        int maxNumTriangles = 5 * (terrainSize.x - 1) * (terrainSize.y - 1) * (terrainSize.z - 1);
        if (triangleBuffer == null || !triangleBuffer.IsValid() || triangleBuffer.count != maxNumTriangles) {
            triangleBuffer = new ComputeBuffer(maxNumTriangles, 36, ComputeBufferType.Append);
        }

        if (countBuffer == null || !countBuffer.IsValid()) {
            countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        }
    }

    private static void SetShaderValues(Vector3Int terrainSize, float isoLevel) {
        if (polygonizer != null) {
            //setting up the compute shader
            polygonizer.SetInt("sizeX", terrainSize.x);
            polygonizer.SetInt("sizeY", terrainSize.y);
            polygonizer.SetInt("sizeZ", terrainSize.z);
            polygonizer.SetFloat("isolevel", isoLevel);
        }
    }

    private static void SetBufferValues(Vector4[] values) {
        points.SetData(values);
        triangleBuffer.SetCounterValue(0);
    }

    private static void RunShader(Vector3Int terrainSize) {
        int kernel = polygonizer.FindKernel("Polygonize");
        polygonizer.SetBuffer(kernel, "points", points);
        polygonizer.SetBuffer(kernel, "triangles", triangleBuffer);
        polygonizer.Dispatch(kernel, Mathf.CeilToInt(terrainSize.x / 4f), Mathf.CeilToInt(terrainSize.y / 4f), Mathf.CeilToInt(terrainSize.z / 2f));

        //getting the data from the compute shader
        // Copy the count.
        ComputeBuffer.CopyCount(triangleBuffer, countBuffer, 0);

        // Retrieve it into array.
        int[] counter = new int[1] { 0 };
        countBuffer.GetData(counter);

        triangles = new Triangle[counter[0]];
        triangleBuffer.GetData(triangles, 0, 0, counter[0]);
    }

    //shoudln't be run every frame
    public static void DestroyBuffers() {
        if (points != null) {
            points.Release();
        }

        if (triangleBuffer != null) {
            triangleBuffer.Release();
        }

        if (countBuffer != null) {
            countBuffer.Release();
        }
    }

    private struct Triangle {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;
        public Vector3 this[int i] {
            get {
                switch (i) {
                    case 0:
                        return pointA;
                    case 1:
                        return pointB;
                    default:
                        return pointC;
                }
            }
        }
    };
}