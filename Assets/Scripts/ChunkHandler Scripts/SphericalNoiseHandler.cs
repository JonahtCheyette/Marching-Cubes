using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalNoiseHandler : HandlerTemplate {
    public SphericalNoiseSettings settings;

    public override void GenerateValues() {
        values = DensityFunction.GenerateSphericalNoiseValues(marchingCubesSettings, center, settings);
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