using UnityEngine;
using UnityEditor;

//says to use this editor for the BaseTerrainHandler Script
[CustomEditor(typeof(ExpierementalTerrainHandler))]
public class ExpierementalTerrainHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the BaseTerrainHandler reference
        ExpierementalTerrainHandler terrainHandler = (ExpierementalTerrainHandler)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Generate")) {
            terrainHandler.GenerateChunks();
        }
    }
}
