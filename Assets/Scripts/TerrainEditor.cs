using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEditor : MonoBehaviour {

    public Vector3Int terrainSize = new Vector3Int(100, 100, 100);
    [Min(0)]
    public float gridSize;
    [Range(0, 1)]
    public float surfaceLevel = 0.5f;
    [Min(0)]
    public float brushSize = 10f;
    public GameObject brush;
    public Camera cam;
    public Material meshMaterial;

    private Vector4[] values;
    private Bounds bounds;
    private float rayDistance;

    private List<TerrainChunk> chunks = new List<TerrainChunk>();
    private ChunkData[] terrainChunkData;

    private bool valuesChangedLastFrame = false;

    void Start() {
        values = new Vector4[terrainSize.x * terrainSize.y * terrainSize.z];
        bounds = new Bounds(gameObject.transform.position, (terrainSize - Vector3.one) * gridSize);

        //initializes the positions, sets all the points values to 0, except the ones on the bottom layer, whose value gets set to 1
        for (int x = 0; x < terrainSize.x; x++) {
            for (int y = 0; y < terrainSize.y; y++) {
                for (int z = 0; z < terrainSize.z; z++) {
                    values[Utility.XYZtoIndex(x, y, z, terrainSize)] = Utility.Vector4FromVector3andValue(Utility.CalculatePointPosition(new Vector3Int(x, y, z), gameObject.transform.position, gridSize, terrainSize), 0);
                    if(y == 0) {
                        values[Utility.XYZtoIndex(x, y, z, terrainSize)].w = 1;
                    }
                }
            }
        }

        if (Camera.current != null) {
            rayDistance = Mathf.Sqrt(bounds.SqrDistance(Camera.current.transform.position));
        }

        terrainChunkData = ChunkGenerator.Generate(values, terrainSize, surfaceLevel, true);
        CreateChunks();
    }

    // Update is called once per frame
    void Update() {
        bool valuesChanged = false;
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);

        ChangeBrushSize();

        float distance;
        if (bounds.IntersectRay(mouseRay, out distance)) {
            if (!brush.activeSelf) {
                brush.SetActive(true);
            }

            ChangeBrushDistance();

            MoveBrush(mouseRay, distance);

            if (Input.GetMouseButton(0)) {
                Vector3 center = mouseRay.GetPoint(rayDistance);
                //left click
                for (int i = 0; i < values.Length; i++) {
                    Vector3 diff = new Vector3(values[i].x, values[i].y, values[i].z) - center;
                    if (diff.x * diff.x + diff.y * diff.y + diff.z * diff.z < brushSize * brushSize) {
                        values[i].w += 0.01f;
                        valuesChanged = true;
                        values[i].w = Utility.Constrain(0, 1, values[i].w);
                    }
                }
            } else if (Input.GetMouseButton(1)) {
                Vector3 center = mouseRay.GetPoint(rayDistance);
                //right click
                for (int i = 0; i < values.Length; i++) {
                    Vector3 diff = new Vector3(values[i].x, values[i].y, values[i].z) - center;
                    if (diff.x * diff.x + diff.y * diff.y + diff.z * diff.z < brushSize * brushSize) {
                        values[i].w -= 0.01f;
                        valuesChanged = true;
                        values[i].w = Utility.Constrain(0, 1, values[i].w);
                    }
                }
            }
        } else {
            if (brush.activeSelf) {
                brush.SetActive(false);
            }
        }

        if (valuesChanged) {
            terrainChunkData = ChunkGenerator.Generate(values, terrainSize, surfaceLevel);
            CreateChunks();
        } else if(valuesChangedLastFrame) {
            ChunkGenerator.DestroyBuffers();
        }

        valuesChangedLastFrame = valuesChanged;
    }

    private void MoveBrush(Ray mouseRay, float distance) {
        //constraining the brush to be affecting the box
        Ray testRay = new Ray(mouseRay.GetPoint(Mathf.Max(terrainSize.x, terrainSize.y, terrainSize.z) * gridSize), mouseRay.direction * -1);
        float testDistance;
        bounds.IntersectRay(testRay, out testDistance);

        Vector3 originDiff = testRay.origin - mouseRay.origin;
        float farDistance = Mathf.Sqrt(originDiff.x * originDiff.x + originDiff.y * originDiff.y + originDiff.z * originDiff.z) - testDistance;

        rayDistance = Utility.Constrain(distance - brushSize / 2, farDistance + brushSize / 2, rayDistance);

        brush.transform.position = mouseRay.GetPoint(rayDistance);
        brush.transform.localScale = Vector3.one * brushSize;
    }

    private void ChangeBrushDistance() {
        if (!Input.GetKey(KeyCode.Z)) {
            if (Input.mouseScrollDelta.y > 0) {
                //scroll up
                rayDistance += 0.5f;
            } else if (Input.mouseScrollDelta.y < 0) {
                //scroll down
                rayDistance -= 0.5f;
            }
        }
    }

    private void ChangeBrushSize() {
        if (Input.GetKey(KeyCode.Z)) {
            if (Input.mouseScrollDelta.y > 0) {
                //scroll up
                brushSize += 0.1f;
            } else if (Input.mouseScrollDelta.y < 0) {
                //scroll down
                brushSize -= 0.1f;
            }
            brushSize = Utility.Constrain(0, (terrainSize.x - 1) * gridSize, brushSize);
        }
    }

    private void CreateChunks() {
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
        }
        chunks.Clear();

        for (int i = 0; i < terrainChunkData.Length; i++) {
            chunks.Add(new TerrainChunk(terrainChunkData[i], gameObject, meshMaterial));
        }
    }

    private void OnValidate() {
        if (terrainSize.x < 2) {
            terrainSize.x = 2;
        }
        if (terrainSize.y < 2) {
            terrainSize.y = 2;
        }
        if (terrainSize.z < 2) {
            terrainSize.z = 2;
        }
    }

    private void OnApplicationQuit() {
        ChunkGenerator.DestroyBuffers();
        DensityFunction.DestroyBuffer();
    }
}
