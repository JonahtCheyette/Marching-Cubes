using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MarchingCubeSettings : ScriptableObject {
    public Vector3Int size = Vector3Int.one * 2;
    [Min(float.Epsilon)]
    public float gridSize = 1;
    public float surfaceLevel = 0.5f;
    public bool usePercentageSurfaceLevel = true;

    public event System.Action OnValuesUpdated;

    private void OnValidate() {
        if (size.x < 2) {
            size.x = 2;
        }
        if (size.y < 2) {
            size.y = 2;
        }
        if (size.z < 2) {
            size.z = 2;
        }

        if (usePercentageSurfaceLevel) {
            if (surfaceLevel < 0) {
                surfaceLevel = 0;
            } else if (surfaceLevel > 1) {
                surfaceLevel = 1;
            }
        }

        if (OnValuesUpdated != null) {
            OnValuesUpdated();
        }
    }
}
