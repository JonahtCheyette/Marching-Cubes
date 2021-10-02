using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpedNoiseHandler : HandlerTemplate {
    public WarpedNoiseSettings settings;

    public override void GenerateValues() {
        values = DensityFunction.GenerateWarpedNoiseValues(marchingCubesSettings, center, settings);
    }

    public override void OnValidate() {
        if (settings != null) {
            settings.OnValuesUpdated -= OnValidate;
            settings.OnValuesUpdated += OnValidate;
        }

        base.OnValidate();
    }

    protected override void UpdateChunksIfAutoUpdateIsOn() {
        UnityEditor.EditorApplication.delayCall -= UpdateChunksIfAutoUpdateIsOn;
        if (marchingCubesSettings != null && settings != null) {
            if (AutoUpdate) {
                GenerateChunks();
            }
        }
    }
}

[System.Serializable]
public struct WarpSettings {
    [Min(0.0001f)]
    public float scale;
    public Vector3 offsetOne;
    public Vector3 offsetTwo;
    public Vector3 offsetThree;
}
