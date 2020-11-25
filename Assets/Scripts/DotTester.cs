//I'm shelving this for now. I will come bach to it once I have an Idea of how to fix it. 

//As of writing, the problem is that this needs to have the variables in it in order to run every kind of density function,
//and I can't figure out a way to do this without having a fuck ton of variables that would need to be assigned in the editor

//so for instance, as of writing, there's only one kind of density function, and also in our Generate Values function we currently have an error
//where we aren't passing in all the necessary values because they aren't declared in this class. I would like to avoid having to declare and set new variables
//for every new kind of density function we have

/*
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

    private float[] dotValues = new float[1023];

    public enum GenerationType {
        noise
    }

    public void TestWithDots() {
        //generating the values and positions
        points = GenerateValues();

        CreateMeshes();

        FindMinAndMaxValue();

        //getting just the values
        GetPointValues(points);

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

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    //creating the icospheres and setting their positions
                    meshes[x + (y * size.x) + (z * size.x * size.y)] = IcoSphere.Create(dotSize);
                    meshPositions[x + (y * size.x) + (z * size.x * size.y)] = Utility.CalculatePointPosition(new Vector3Int(x, y, z), center, gridSize, size);
                }
            }
        }
    }

    private Vector4[] GenerateValues() {
        if(generationType == GenerationType.noise) {
            return DensityFunction.GenerateNoiseValues(size, gridSize, center);
        } else {
            return new Vector4[0];
        }
    }

    private void SetMaterialValues() {
        Material mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetFloatArray("values", dotValues);
        mat.SetFloat("gridSize", gridSize);
        mat.SetVector("size", new Vector4(size.x, size.y, size.z));
        mat.SetVector("center", new Vector4(center.x, center.y, center.z));
        mat.SetFloat("max", max);
        mat.SetFloat("min", min);
    }

    private void GetPointValues(Vector4[] points) {
        //getting just the values
        //is 1023 elements long just to prevent the shader capping the size of the array when we pass in an array larger than the previous one
        dotValues = new float[1023];
        for (int i = 0; i < points.Length; i++) {
            dotValues[i] = points[i].w;
        }
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

        //clamping the values because shaders can only hold so many values in their arrays
        while (size.x * size.y * size.z > 1023) {
            if (size.x > size.y && size.x > size.z && size.x > 2) {
                size.x--;
            } else if (size.y >= size.x && size.y > size.z && size.y > 2) {
                size.y--;
            } else {
                size.z--;
            }
        }
    }
}*/
