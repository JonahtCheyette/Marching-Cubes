using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//just a class to put some useful functions for the rest of the program
public static class Utility {
    public static int XYZtoIndex(int x, int y, int z, Vector3Int maxNumOnAxis) {
        return z * maxNumOnAxis.y * maxNumOnAxis.x + y * maxNumOnAxis.x + x;
    }

    public static Vector4 Vector4FromVector3andValue(Vector3 vec, float val) {
        return new Vector4(vec.x, vec.y, vec.z, val);
    }

    //calculates a position in space based on a coord in the values array
    public static Vector3 CalculatePointPosition(Vector3Int coord, Vector3 center, float gridSize, Vector3Int size) {
        //we have to subtract 0.5 to avoid off-by-one errors
        Vector3 halfSize = (new Vector3(size.x, size.y, size.z) / 2f) - (Vector3.one * 0.5f);
        return center + (coord - halfSize) * gridSize;
    }

    //pretty standard mesh combining thing, assumes all the meshes are properly aligned
    public static Mesh CombineMeshes(Mesh[] meshes) {
        CombineInstance[] combiners = new CombineInstance[meshes.Length];

        for (int i = 0; i < meshes.Length; i++) {
            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = meshes[i];
            Matrix4x4 position = Matrix4x4.identity;
            combiners[i].transform = position;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combiners);

        return finalMesh;
    }

    //same as above, but the meshes aren't already aligned, so instead takes in an array of positions as well
    public static Mesh CombineMeshes(Mesh[] meshes, Vector3[] positions) {
        CombineInstance[] combiners = new CombineInstance[meshes.Length];

        for (int i = 0; i < meshes.Length; i++) {
            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = meshes[i];
            Matrix4x4 position = Matrix4x4.identity;
            position.SetColumn(3, new Vector4(positions[i].x, positions[i].y, positions[i].z, 0));
            combiners[i].transform = position;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combiners);

        return finalMesh;
    }

    /*
    //basically just for the dot tester script
    //same as above, but assuming it's just one mesh being copied at every position, and also assigns uvs
    public static Mesh CombineMeshes(Mesh[] mesh, Vector3[] positions, float[] pointValues) {
        CombineInstance[] combiners = new CombineInstance[positions.Length];

        for (int i = 0; i < positions.Length; i++) {
            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = mesh[0];
            Vector2[] uvs = new Vector2[12];
            for (int j = 0; j < 12; j++) {
                uvs[j] = Vector2.one * pointValues[i];
            }
            combiners[i].mesh.uv = uvs;
            Matrix4x4 position = Matrix4x4.identity;
            position.SetColumn(3, new Vector4(positions[i].x, positions[i].y, positions[i].z, 0));
            combiners[i].transform = position;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combiners);

        return finalMesh;
    }*/

    public static float Interpolate(float min, float max, float percentage) {
        return min + ((max - min) * percentage);
    }

    public static float Constrain(float min, float max, float val) {
        if(val > max) {
            return max;
        }
        if(val < min) {
            return min;
        }
        return val;
    }
}
