using UnityEngine;
using UnityEditor;

//says to use this editor for the ExpierementalTerrainHandler Script
[CustomEditor(typeof(ExpierementalTerrainHandler))]
public class ExpierementalTerrainHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the ExpierementalTerrainHandler reference
        ExpierementalTerrainHandler terrainHandler = (ExpierementalTerrainHandler)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Generate")) {
            terrainHandler.GenerateChunks();
        }
    }
}
