using UnityEngine;

public class TerrainChunk {
    private GameObject gameObject;
    private MeshFilter filter;
    private MeshRenderer renderer;

    public TerrainChunk(ChunkData data, GameObject parent, Material mat) {
        gameObject = new GameObject("Terrain Chunk");
        gameObject.transform.parent = parent.transform;

        Vector3 avgCenter = new Vector3();
        for (var i = 0; i < data.vertices.Length; i++) {
            avgCenter += data.vertices[i];
        }
        avgCenter /= data.vertices.Length;
        gameObject.transform.position = avgCenter;

        for (var i = 0; i < data.vertices.Length; i++) {
            data.vertices[i] -= avgCenter;
        }
        filter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = data.vertices;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();
        if (data.hasNormals) {
            mesh.normals = data.normals;
        }
        filter.sharedMesh = mesh;
        renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = mat;
    }

    public void Destroy() {
        Object.DestroyImmediate(gameObject);
    }

    public bool HasBeenDeleted() {
        return gameObject == null;
    }
}

public struct ChunkData {
    public Vector3[] vertices;
    public int[] triangles;
    public bool hasNormals;
    public Vector3[] normals;

    public ChunkData(Vector3[] vertices, int[] triangles) {
        this.vertices = vertices;
        this.triangles = triangles;
        hasNormals = false;
        normals = new Vector3[0];
    }

    public ChunkData(Vector3[] vertices, int[] triangles, Vector3[] normals) {
        this.vertices = vertices;
        this.triangles = triangles;
        hasNormals = true;
        this.normals = normals;
    }
}

