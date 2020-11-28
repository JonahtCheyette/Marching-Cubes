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
        filter.sharedMesh = mesh;
        renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = mat;
    }

    //for potential chunking for an infinite, procedurally generated terrain
    /*
    public TerrainChunk(ChunkData data, GameObject parent, Material mat) {
        gameObject = new GameObject("Terrain Chunk");
        gameObject.transform.parent = parent.transform;
        gameObject.transform.position = data.center;
        for (var i = 0; i < data.vertices.Length; i++) {
            data.vertices[i] -= data.center;
        }
        filter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = data.vertices;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();
        filter.sharedMesh = mesh;
        renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = mat;
    }

    public void Remake(ChunkData data) {
        gameObject.transform.position = data.center;
        for (var i = 0; i < data.vertices.Length; i++) {
            data.vertices[i] -= data.center;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = data.vertices;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();
        filter.sharedMesh = mesh;
    }*/

    public void Destroy() {
        Object.DestroyImmediate(gameObject);
    }

    public bool HasBeenDeleted() {
        return gameObject == null;
    }
}

//for potential chunking for an infinite, procedurally generated terrain
/*
public struct ChunkData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3 center;

    public ChunkData(Vector3[] vertices, int[] triangles,Vector3 center) {
        this.vertices = vertices;
        this.triangles = triangles;
        this.center = center;
    }
}*/

public struct ChunkData {
    public Vector3[] vertices;
    public int[] triangles;

    public ChunkData(Vector3[] vertices, int[] triangles) {
        this.vertices = vertices;
        this.triangles = triangles;
    }
}

