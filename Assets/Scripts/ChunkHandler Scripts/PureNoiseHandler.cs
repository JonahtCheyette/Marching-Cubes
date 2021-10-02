using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PureNoiseHandler : HandlerTemplate {
    public PureNoiseSettings settings;

    public override void GenerateValues() {
        values = DensityFunction.GenerateNoiseValues(marchingCubesSettings, center, settings);
    }

    public override void OnValidate() {
        if (settings != null) {
            settings.OnValuesUpdated -= OnValidate;
            settings.OnValuesUpdated += UpdateChunksIfAutoUpdateIsOn;
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
