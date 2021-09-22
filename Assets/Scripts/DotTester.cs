//I'm shelving this for now. I will come bach to it once I have an Idea of how to fix it. 

//As of writing, the problem is that this needs to have the variables in it in order to run every kind of density function,
//and I can't figure out a way to do this without having a fuck ton of variables that would need to be assigned in the editor

//so for instance, as of writing, there's only one kind of density function, and also in our Generate Values function we currently have an error
//where we aren't passing in all the necessary values because they aren't declared in this class. I would like to avoid having to declare and set new variables
//for every new kind of density function we have


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTester : MonoBehaviour {
    public Vector3Int size;

    [Range(0, 1)]
    public float dotSize;

    public float gridSize;

    public Vector3 center;

    public GenerationType generationType;

    private Vector4[] points;

    private Mesh[] meshes;
    private Vector3[] meshPositions;

    private float min;
    private float max;

    public enum GenerationType {
        noise
    }

    public void TestWithDots() {
        //generating the values and positions
        points = GenerateValues();

        CreateMeshes();

        FindMinAndMaxValue();

        SetMaterialValues();

        //applying the mesh
        gameObject.GetComponent<MeshFilter>().sharedMesh = Utility.CombineMeshes(meshes, meshPositions);
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
    }

    private void CreateMeshes() {
        meshes = new Mesh[points.Length];
        meshPositions = new Vector3[points.Length];
        Vector2[] uvs = new Vector2[12];

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    int i = x + (y * size.x) + (z * size.x * size.y);
                    //creating the icospheres and setting their positions
                    meshes[i] = IcoSphere.Create(dotSize);

                    for(int j = 0; j < 12; j++) {
                        uvs[j] = Vector2.one * points[i].w;
                    }
                    meshes[i].uv = uvs;

                    meshPositions[i] = Utility.CalculatePointPosition(new Vector3Int(x, y, z), center, gridSize, size);
                }
            }
        }
    }

    private Vector4[] GenerateValues() {
        if(generationType == GenerationType.noise) {
            return DensityFunction.GenerateNoiseValues(size, gridSize, center, 20, 8, 0.387f, 2, 1000);
        } else {
            return new Vector4[0];
        }
        return new Vector4[size.x * size.y * size.z];
    }

    private void SetMaterialValues() {
        Material mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetFloat("max", max);
        mat.SetFloat("min", min);
    }

    private void OnValidate() {
        if(size.x < 2) {
            size.x = 2;
        }
        if (size.y < 2) {
            size.y = 2;
        }
        if (size.z < 2) {
            size.z = 2;
        }
    }
}
