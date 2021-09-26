using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkGenerator {
    private static ComputeShader polygonizer = (ComputeShader)Resources.Load("ComputeShaders/Polygonize");
    private static ComputeShader polygonizerWithEdges = (ComputeShader)Resources.Load("ComputeShaders/PolygonizeWithEdges");

    private static ComputeBuffer points;
    private static ComputeBuffer triangleBuffer;
    private static ComputeBuffer triangleWithEdgesBuffer;
    // Buffer to store count in.
    private static ComputeBuffer countBuffer;

    private static Triangle[] triangles;
    private static TriangleWithEdgeIndicies[] trianglesWithEdges;

    public static ChunkData[] Generate(Vector4[] values, Vector3Int terrainSize, float isoLevel, bool useFlatshading, bool deleteBuffers = false) {
        SetShaderValues(terrainSize, isoLevel, useFlatshading);

        GenerateTriangles(values, terrainSize, deleteBuffers, useFlatshading);

        return CreateTerrainChunks(useFlatshading);
    }

    private static void GenerateTriangles(Vector4[] values, Vector3Int terrainSize, bool deleteBuffers, bool useFlatshading) {
        CreateBuffers(values.Length, terrainSize, useFlatshading);

        SetBufferValues(values, useFlatshading);

        RunShader(terrainSize, useFlatshading);

        if (!Application.isPlaying || deleteBuffers) {
            DestroyBuffers();
        }
    }

    //creates terrainchunks to make the terrain
    private static ChunkData[] CreateTerrainChunks(bool useFlatshading) {
        if (useFlatshading) {
            int numTris = triangles.Length;
            return CreateFlatShadedChunkData(numTris);
        } else {
            int numTris = trianglesWithEdges.Length;
            return CreateSmoothShadedChunkData(numTris);
        }
    }

    private static ChunkData[] CreateFlatShadedChunkData(int numTris) {
        List<ChunkData> chunks = new List<ChunkData>();

        int index = 0;
        //the number of tris / the number of tris in one mesh
        int numChunks = Mathf.CeilToInt(numTris / 21845f);
        for (int i = 0; i < numChunks; i++) {
            List<Vector3> chunkVertices = new List<Vector3>();
            List<int> chunkTriangles = new List<int>();
            for (int j = index; j < index + 21845; j++) {
                if (j < numTris) {
                    for (int k = 0; k < 3; k++) {
                        chunkVertices.Add(triangles[j][k]);
                        chunkTriangles.Add(chunkVertices.Count - 1);
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

    private static ChunkData[] CreateSmoothShadedChunkData(int numTris) {
        List<ChunkData> chunks = new List<ChunkData>();
        
        //removing duplicate vertices
        Dictionary<Vector2Int, int> edgeToVertexIndex = new Dictionary<Vector2Int, int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                Vector2Int edge;
                if (j == 0) {
                    edge = trianglesWithEdges[i].edgeA;
                } else if (j == 1) {
                    edge = trianglesWithEdges[i].edgeB;
                } else {
                    edge = trianglesWithEdges[i].edgeC;
                }
                if (edgeToVertexIndex.ContainsKey(edge)) {
                    int vertexIndex;
                    edgeToVertexIndex.TryGetValue(edge, out vertexIndex);
                    triangles.Add(vertexIndex);
                    normals[vertexIndex] += trianglesWithEdges[i].normal;
                } else {
                    vertices.Add(trianglesWithEdges[i][j]);
                    normals.Add(trianglesWithEdges[i].normal);
                    triangles.Add(vertices.Count - 1);
                    edgeToVertexIndex.Add(edge, vertices.Count - 1);
                }
            }
        }
        for (int i = 0; i < normals.Count; i++) {
            normals[i] = Vector3.Normalize(normals[i]);
        }


        //dividing the triangles into chunks
        List<Vector3> chunkVertices = new List<Vector3>();
        List<Vector3> chunkNormals = new List<Vector3>();
        List<int> chunkTriangles = new List<int>();
        Dictionary<int, int> globalIndexToChunkIndex = new Dictionary<int, int>();
        for (int i = 0; i < triangles.Count; i += 3) {
            bool wouldGoOverVertexLimit = false;
            if (chunkVertices.Count >= 65533) {
                int numNewVertices = 0;
                int newVertexLimit = 65535 - chunkVertices.Count;

                for (int j = 0; j < 3; j++) {
                    if (!globalIndexToChunkIndex.ContainsKey(triangles[i + j])) {
                        numNewVertices++;
                    }
                }

                if (numNewVertices > newVertexLimit) {
                    wouldGoOverVertexLimit = true;
                }
            }

            if (!wouldGoOverVertexLimit) {
                for (int j = 0; j < 3; j++) {
                    int vertexIndex = triangles[i + j];
                    if (globalIndexToChunkIndex.ContainsKey(vertexIndex)) {
                        int chunkVertexIndex;
                        globalIndexToChunkIndex.TryGetValue(vertexIndex, out chunkVertexIndex);
                        chunkTriangles.Add(chunkVertexIndex);
                    } else {
                        chunkVertices.Add(vertices[vertexIndex]);
                        chunkNormals.Add(normals[vertexIndex]);
                        chunkTriangles.Add(chunkVertices.Count - 1);
                        globalIndexToChunkIndex.Add(vertexIndex, chunkVertices.Count - 1);
                    }
                }
            } else {
                chunks.Add(new ChunkData(chunkVertices.ToArray(), chunkTriangles.ToArray(), chunkNormals.ToArray()));
                chunkNormals = new List<Vector3>();
                chunkTriangles = new List<int>();
                chunkVertices = new List<Vector3>();
                globalIndexToChunkIndex = new Dictionary<int, int>();
                for (int j = 0; j < 3; j++) {
                    int vertexIndex = triangles[i + j];
                    chunkVertices.Add(vertices[vertexIndex]);
                    chunkNormals.Add(normals[vertexIndex]);
                    chunkTriangles.Add(chunkVertices.Count - 1);
                    globalIndexToChunkIndex.Add(vertexIndex, chunkVertices.Count - 1);
                }
            }
        }
        chunks.Add(new ChunkData(chunkVertices.ToArray(), chunkTriangles.ToArray(), chunkNormals.ToArray()));

        return chunks.ToArray();
    }

    private static void CreateBuffers(int numPoints, Vector3Int terrainSize, bool useFlatShading) {
        if (points == null || !points.IsValid() || points.count != numPoints) {
            points = new ComputeBuffer(numPoints, sizeof(float) * 4);
        }

        int maxNumTriangles = 5 * (terrainSize.x - 1) * (terrainSize.y - 1) * (terrainSize.z - 1);
        if (useFlatShading) {
            if (triangleBuffer == null || !triangleBuffer.IsValid() || triangleBuffer.count != maxNumTriangles) {
                triangleBuffer = new ComputeBuffer(maxNumTriangles, sizeof(float) * 9, ComputeBufferType.Append);
            }
        } else {
            if (triangleWithEdgesBuffer == null || !triangleWithEdgesBuffer.IsValid() || triangleWithEdgesBuffer.count != maxNumTriangles) {
                triangleWithEdgesBuffer = new ComputeBuffer(maxNumTriangles, sizeof(float) * 12 + sizeof(int) * 6, ComputeBufferType.Append);
            }
        }

        if (countBuffer == null || !countBuffer.IsValid()) {
            countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        }
    }

    private static void SetShaderValues(Vector3Int terrainSize, float isoLevel, bool useFlatshading) {
        if (useFlatshading) {
            if (polygonizer != null) {
                //setting up the compute shader
                polygonizer.SetInt("sizeX", terrainSize.x);
                polygonizer.SetInt("sizeY", terrainSize.y);
                polygonizer.SetInt("sizeZ", terrainSize.z);
                polygonizer.SetFloat("isolevel", isoLevel);
            }
        } else {
            if (polygonizerWithEdges != null) {
                //setting up the compute shader
                polygonizerWithEdges.SetInt("sizeX", terrainSize.x);
                polygonizerWithEdges.SetInt("sizeY", terrainSize.y);
                polygonizerWithEdges.SetInt("sizeZ", terrainSize.z);
                polygonizerWithEdges.SetFloat("isolevel", isoLevel);
            }
        }
    }

    private static void SetBufferValues(Vector4[] values, bool useFlatshading) {
        points.SetData(values);
        if (useFlatshading) {
            triangleBuffer.SetCounterValue(0);
        } else {
            triangleWithEdgesBuffer.SetCounterValue(0);
        }
    }

    private static void RunShader(Vector3Int terrainSize, bool useFlatshading) {
        if (useFlatshading) {
            int kernel = polygonizer.FindKernel("Polygonize");
            polygonizer.SetBuffer(kernel, "points", points);
            polygonizer.SetBuffer(kernel, "triangles", triangleBuffer);
            polygonizer.Dispatch(kernel, Mathf.CeilToInt((terrainSize.x - 1) / 4f), Mathf.CeilToInt((terrainSize.y - 1) / 4f), Mathf.CeilToInt((terrainSize.z - 1) / 2f));
            
            //getting the data from the compute shader
            // Copy the count.
            ComputeBuffer.CopyCount(triangleBuffer, countBuffer, 0);
        } else {
            int kernel = polygonizerWithEdges.FindKernel("Polygonize");
            polygonizerWithEdges.SetBuffer(kernel, "points", points);
            polygonizerWithEdges.SetBuffer(kernel, "triangles", triangleWithEdgesBuffer);
            polygonizerWithEdges.Dispatch(kernel, Mathf.CeilToInt((terrainSize.x - 1) / 4f), Mathf.CeilToInt((terrainSize.y - 1) / 4f), Mathf.CeilToInt((terrainSize.z - 1) / 2f));

            //getting the data from the compute shader
            // Copy the count.
            ComputeBuffer.CopyCount(triangleWithEdgesBuffer, countBuffer, 0);
        }

        // Retrieve it into array.
        int[] counter = new int[1] { 0 };
        countBuffer.GetData(counter);

        if (useFlatshading) {
            triangles = new Triangle[counter[0]];
            triangleBuffer.GetData(triangles, 0, 0, counter[0]);
        } else {
            trianglesWithEdges = new TriangleWithEdgeIndicies[counter[0]];
            triangleWithEdgesBuffer.GetData(trianglesWithEdges, 0, 0, counter[0]);
        }
    }

    //shoudln't be run every frame
    public static void DestroyBuffers() {
        if (points != null) {
            points.Release();
        }

        if (triangleBuffer != null) {
            triangleBuffer.Release();
        }

        if (triangleWithEdgesBuffer != null) {
            triangleWithEdgesBuffer.Release();
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
            set {
                switch (i) {
                    case 0:
                        pointA = value;
                        return;
                    case 1:
                        pointB = value;
                        return;
                    default:
                        pointC = value;
                        return;
                }
            }
        }
    };

    private struct TriangleWithEdgeIndicies {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;

        public Vector2Int edgeA;
        public Vector2Int edgeB;
        public Vector2Int edgeC;

        public Vector3 normal;

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