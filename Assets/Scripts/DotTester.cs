using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTester : MonoBehaviour {
    public Vector3 center;
    [Range(0, 1)]
    public float dotSize;
    public bool autoUpdate = false;
    public bool showMinAndMaxValues = false;
    public MarchingCubeSettings marchingCubeSettings;
    public NoiseSettings settings;

    private Vector4[] points;

    private Vector3[] meshPositions;
    float[] values;

    private Vector3[] sphereVertices;
    private int[] sphereTriangles;

    private float min;
    private float max;
    private Material mat;

    public void TestWithDots() {
        SetUpMeshVetsAndTriangles();
        //generating the values and positions
        points = GenerateValues();

        if (marchingCubeSettings != null && settings != null) {
            CreateMeshes();
            FindMinAndMaxValue();
            SetUpMaterial();
            SetMaterialValues();
            //applying the meshes
            AssignMeshes();
        }
    }

    private void SetUpMeshVetsAndTriangles() {
        if(sphereVertices == null || sphereTriangles == null || sphereVertices.Length != 12 || sphereTriangles.Length != 60) {
            Mesh sphere = IcoSphere.Create(1);
            sphereVertices = sphere.vertices;
            sphereTriangles = sphere.triangles;
        }
    }

    private void SetUpMaterial() {
        if (mat == null) {
            mat = new Material(Shader.Find("Unlit/DotShader"));
        }
    }

    private void FindMinAndMaxValue() {
        max = float.MinValue;
        min = float.MaxValue;
        for (int i = 0; i < points.Length; i++) {
            if (points[i].w < min) {
                min = points[i].w;
            }
            if (points[i].w > max) {
                max = points[i].w;
            }
        }

        if (showMinAndMaxValues) {
            print("Min: " + min);
            print("Max: " + max);
        }
    }

    private void CreateMeshes() {
        meshPositions = new Vector3[points.Length];
        values = new float[points.Length];

        for (int x = 0; x < marchingCubeSettings.size.x; x++) {
            for (int y = 0; y < marchingCubeSettings.size.y; y++) {
                for (int z = 0; z < marchingCubeSettings.size.z; z++) {
                    int i = x + (y * marchingCubeSettings.size.x) + (z * marchingCubeSettings.size.x * marchingCubeSettings.size.y);
                    values[i] = points[i].w;
                    meshPositions[i] = Utility.CalculatePointPosition(new Vector3Int(x, y, z), center, marchingCubeSettings.gridSize, marchingCubeSettings.size);
                }
            }
        }


    }

    private void AssignMeshes() {
        EmptyChildren();

        int numIcoSpheresPerMesh = Mathf.FloorToInt(65535f / 12f);
        int numMeshes = Mathf.CeilToInt(points.Length / (float)numIcoSpheresPerMesh);

        int startIndex = 0;

        for (int i = 0; i < numMeshes; i++) {
            GameObject newChild = new GameObject("Dots");
            MeshRenderer renderer = newChild.AddComponent<MeshRenderer>();
            MeshFilter filter = newChild.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();

            int numSpheres = i != numMeshes - 1 ? numIcoSpheresPerMesh : points.Length - startIndex;

            Vector3[] meshVertices = new Vector3[numSpheres * 12];
            Vector2[] meshUvs = new Vector2[numSpheres * 12];
            int[] meshTriangles = new int[numSpheres * 60];
            for (int j = startIndex; j < startIndex + numSpheres; j++) {
                int vertexStartIndex = (j - startIndex) * 12;
                int triangleStartIndex = (j - startIndex) * 60;
                for (int k = 0; k < 12; k++) {
                    meshVertices[vertexStartIndex + k] = meshPositions[j] + sphereVertices[k] * dotSize;
                    meshUvs[vertexStartIndex + k] = Vector2.one * values[j];
                }
                for (int k = 0; k < 60; k++) {
                    meshTriangles[triangleStartIndex + k] = sphereTriangles[k] + vertexStartIndex;
                }
            }

            mesh.vertices = meshVertices;
            mesh.uv = meshUvs;
            mesh.triangles = meshTriangles;

            filter.sharedMesh = mesh;
            renderer.sharedMaterial = mat;
            newChild.transform.parent = gameObject.transform;

            startIndex += numIcoSpheresPerMesh;
        }
    }

    private void EmptyChildren() {
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
        }
    }

    private Vector4[] GenerateValues() {
        if (marchingCubeSettings != null) {
            if (settings != null) {
                System.Type type = settings.GetType();
                if (type.Equals(typeof(PureNoiseSettings))) {
                    return DensityFunction.GenerateNoiseValues(marchingCubeSettings, center, (PureNoiseSettings)settings);
                } else if (type.Equals(typeof(BestTerrainSettings))) {
                    return DensityFunction.GenerateBestTerrainValues(marchingCubeSettings, center, (BestTerrainSettings)settings);
                } else if (type.Equals(typeof(ExpierementalTerrainSettings))) {
                    return DensityFunction.GenerateExpierementalTerrainValues(marchingCubeSettings, center, (ExpierementalTerrainSettings)settings);
                } else if (type.Equals(typeof(OverthoughtTerrainSettings))) {
                    return DensityFunction.GenerateOverthoughtTerrainValues(marchingCubeSettings, center, (OverthoughtTerrainSettings)settings);
                } else if (type.Equals(typeof(SphericalNoiseSettings))) {
                    return DensityFunction.GenerateSphericalNoiseValues(marchingCubeSettings, center, (SphericalNoiseSettings)settings);
                } else if (type.Equals(typeof(WarpedNoiseSettings))) {
                    return DensityFunction.GenerateWarpedNoiseValues(marchingCubeSettings, center, (WarpedNoiseSettings)settings);
                } else {
                    print("the type of noiseSettings put into the dotTester has not yet been implemented");
                    return new Vector4[marchingCubeSettings.size.x * marchingCubeSettings.size.y * marchingCubeSettings.size.z];
                }
            } else {
                print("no NoiseSettings has been put into the dotTester");
                return new Vector4[marchingCubeSettings.size.x * marchingCubeSettings.size.y * marchingCubeSettings.size.z];
            }
        } else {
            print("no marchingCubeSettings has been put into the dotTester");
            return new Vector4[0];
        }
    }

    private void SetMaterialValues() {
        mat.SetFloat("max", max);
        mat.SetFloat("min", min);
    }

    private void OnValidate() {
        if (marchingCubeSettings != null && autoUpdate) {
            marchingCubeSettings.OnValuesUpdated -= TestWithDots;
            marchingCubeSettings.OnValuesUpdated += TestWithDots;
        }
    }
}
