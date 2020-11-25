using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkGenerator {
    private static ComputeShader polygonizer = (ComputeShader)Resources.Load("ComputeShaders/Polygonize");

    private static ComputeBuffer points;
    private static ComputeBuffer triangleBuffer;
    // Buffer to store count in.
    private static ComputeBuffer countBuffer;

    private static List<Triangle>[] triangles;

    public static ChunkData[] Generate(Vector4[] values, Vector3Int terrainSize, Vector3Int chunkSize, Vector3 center, float gridSize, float isoLevel) {
        SetShaderValues(terrainSize, chunkSize, isoLevel);

        
        GenerateTriangles(values, terrainSize, chunkSize);

        return CreateTerrainChunks(terrainSize, chunkSize, center, gridSize);
    }

    private static void GenerateTriangles(Vector4[] values, Vector3Int terrainSize, Vector3Int chunkSize) {
        CreateBuffers(values.Length, terrainSize);

        Vector3 numChunks = new Vector3(Mathf.Ceil((terrainSize.x - 1) / (float)(chunkSize.x - 1)), Mathf.Ceil((terrainSize.y - 1) / (float)(chunkSize.y - 1)), Mathf.Ceil((terrainSize.z - 1) / (float)(chunkSize.z - 1)));
        SetBufferValues(values);

        TriangleRaw[] trianglesRaw = RunShader(terrainSize);

        triangles = new List<Triangle>[(int)(numChunks.x * numChunks.y * numChunks.z)];

        for (int i = 0; i < (int)(numChunks.x * numChunks.y * numChunks.z); i++) {
            triangles[i] = new List<Triangle>();
        }
        
        foreach (TriangleRaw rawTriangle in trianglesRaw) {
            triangles[rawTriangle.chunk].Add(rawTriangle.ToTriangle());
        }

        if (!Application.isPlaying) {
            DestroyBuffers();
        }
    }

    //creates terrainchunks to fill up the specified terrain size
    private static ChunkData[] CreateTerrainChunks(Vector3Int terrainSize, Vector3Int chunkSize, Vector3 center, float gridSize) {
        List<ChunkData> chunks = new List<ChunkData>();

        Vector3 numChunks = new Vector3(Mathf.Ceil((terrainSize.x - 1) / (float)(chunkSize.x - 1)), Mathf.Ceil((terrainSize.y - 1) / (float)(chunkSize.y - 1)), Mathf.Ceil((terrainSize.z - 1) / (float)(chunkSize.z - 1)));

        for (int x = 0; x < numChunks.x; x++) {
            for (int y = 0; y < numChunks.y; y++) {
                for (int z = 0; z < numChunks.z; z++) {
                    //corner means top back left corner, in other words, the point that this chunk's values start at when copying them from the generated values
                    int cornerX = x * (chunkSize.x - 1);
                    int cornerY = y * (chunkSize.x - 1);
                    int cornerZ = z * (chunkSize.x - 1);

                    Vector3Int chunkSizeCorrected = chunkSize;
                    if(chunkSizeCorrected.x + cornerX > terrainSize.x) {
                        chunkSizeCorrected.x -= chunkSizeCorrected.x + cornerX - terrainSize.x;
                    }
                    if (chunkSizeCorrected.y + cornerY > terrainSize.y) {
                        chunkSizeCorrected.y -= chunkSizeCorrected.y + cornerY - terrainSize.y;
                    }
                    if (chunkSizeCorrected.z + cornerZ > terrainSize.z) {
                        chunkSizeCorrected.z -= chunkSizeCorrected.z + cornerZ - terrainSize.z;
                    }

                    int chunkIndex = (int)(x + y * numChunks.x + z * numChunks.x * numChunks.y);

                    Vector3 chunkCenter = center + gridSize * (new Vector3(cornerX, cornerY, cornerZ) + ((chunkSizeCorrected - Vector3.one) / 2f) - ((new Vector3(terrainSize.x, terrainSize.y, terrainSize.z) - Vector3.one) / 2f));
                    List<Vector3> chunkVertices = new List<Vector3>();
                    List<int> chunkTriangles = new List<int>();
                    

                    foreach (Triangle triangle in triangles[chunkIndex]) {
                        for (int i = 0; i < 3; i++) {
                            chunkVertices.Add(triangle[i]);
                            chunkTriangles.Add(chunkVertices.Count - 1);

                            //this piece of code is so inefficient, but it gets rid of duplicates. Room for improvement! maybe classify vertices by edge in the polygonizer.
                            //plus, it leaves seams in the terrain due to lighting errors
                            /*
                            if (chunkVertices.Contains(triangle[i])) {
                                chunkTriangles.Add(chunkVertices.IndexOf(triangle[i]));
                            } else {
                                chunkVertices.Add(triangle[i]);
                                chunkTriangles.Add(chunkVertices.Count - 1);
                            }*/
                        }
                    }

                    chunks.Add(new ChunkData(chunkVertices.ToArray(), chunkTriangles.ToArray(), chunkCenter));
                }
            }
        }

        return chunks.ToArray();
    }

    private static void CreateBuffers(int numPoints, Vector3Int terrainSize) {;
        if (points == null || !points.IsValid() || points.count != numPoints) {
            points = new ComputeBuffer(numPoints, 16);
        }

        int maxNumTriangles = 5 * (terrainSize.x - 1) * (terrainSize.y - 1) * (terrainSize.z - 1);
        if (triangleBuffer == null || !triangleBuffer.IsValid() || triangleBuffer.count != maxNumTriangles) {
            triangleBuffer = new ComputeBuffer(maxNumTriangles, 40, ComputeBufferType.Append);
        }

        if (countBuffer == null || !countBuffer.IsValid()) {
            countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        }
    }

    private static void SetShaderValues(Vector3Int terrainSize, Vector3Int chunkSize, float isoLevel) {
        if (polygonizer != null) {
            //setting up the compute shader
            polygonizer.SetInt("sizeX", terrainSize.x);
            polygonizer.SetInt("sizeY", terrainSize.y);
            polygonizer.SetInt("sizeZ", terrainSize.z);
            polygonizer.SetFloat("isolevel", isoLevel);
            polygonizer.SetInt("chunkSizeX", chunkSize.x);
            polygonizer.SetInt("chunkSizeY", chunkSize.y);
            polygonizer.SetInt("chunkSizeZ", chunkSize.z);
            polygonizer.SetInt("numChunksX", (int)Mathf.Ceil((terrainSize.x - 1) / (float)(chunkSize.x - 1)));
            polygonizer.SetInt("numChunksY", (int)Mathf.Ceil((terrainSize.y - 1) / (float)(chunkSize.y - 1)));
            polygonizer.SetInt("numChunksZ", (int)Mathf.Ceil((terrainSize.z - 1) / (float)(chunkSize.z - 1)));
        }
    }

    private static  void SetBufferValues(Vector4[] values) {
        points.SetData(values);
        triangleBuffer.SetCounterValue(0);
    }

    private static TriangleRaw[] RunShader(Vector3Int terrainSize) {
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

        TriangleRaw[] triangles = new TriangleRaw[counter[0]];
        triangleBuffer.GetData(triangles, 0, 0, counter[0]);

        return triangles;
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

    private struct TriangleRaw {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;
        public int chunk;
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

        public Triangle ToTriangle() {
            Triangle tri = new Triangle();
            tri.pointA = pointA;
            tri.pointB = pointB;
            tri.pointC = pointC;
            return tri;
        }
    };

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