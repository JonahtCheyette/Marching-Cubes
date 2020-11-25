using UnityEngine;
using UnityEditor;

//says to use this editor for the NoiseDensityChunkHandler Script
[CustomEditor(typeof(OverthoughtTerrainHandler))]
public class OverthoughtTerrainHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the chunkGenerator reference
        OverthoughtTerrainHandler terrainHandler = (OverthoughtTerrainHandler)target;
        DrawDefaultInspector();

        //generates a chunk
        if (GUILayout.Button("Generate")) {
            terrainHandler.GenerateChunks();
        }
    }
}
