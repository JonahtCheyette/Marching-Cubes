using UnityEngine;
using UnityEditor;

//says to use this editor for the BestTerrainHandler Script
[CustomEditor(typeof(BestTerrainHandler))]
public class BestTerrainHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the BestTerrainHandler reference
        BestTerrainHandler terrainHandler = (BestTerrainHandler)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Generate")) {
            terrainHandler.GenerateChunks();
        }
    }
}
