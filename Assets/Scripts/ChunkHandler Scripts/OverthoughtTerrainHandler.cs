using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverthoughtTerrainHandler : HandlerTemplate {
    public OverthoughtTerrainSettings terrainSettings;

    public override void GenerateValues() {
        values = DensityFunction.GenerateOverthoughtTerrainValues(marchingCubesSettings, center, terrainSettings);
    }

    public override void OnValidate() {
        if (terrainSettings != null) {
            terrainSettings.OnValuesUpdated -= OnValidate;
            terrainSettings.OnValuesUpdated += OnValidate;
        }

        base.OnValidate();
    }

    protected override void UpdateChunksIfAutoUpdateIsOn() {
        UnityEditor.EditorApplication.delayCall -= UpdateChunksIfAutoUpdateIsOn;
        if (marchingCubesSettings != null && terrainSettings != null) {
            if (AutoUpdate) {
                GenerateChunks();
            }
        }
    }
}
