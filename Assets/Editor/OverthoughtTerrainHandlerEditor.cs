using UnityEngine;
using UnityEditor;

//says to use this editor for the OverthoughtTerrainHandler Script
[CustomEditor(typeof(OverthoughtTerrainHandler))]
public class OverthoughtTerrainHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the OverthoughtTerrainHandler reference
        OverthoughtTerrainHandler terrainHandler = (OverthoughtTerrainHandler)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Generate")) {
            terrainHandler.GenerateChunks();
        }
    }
}
