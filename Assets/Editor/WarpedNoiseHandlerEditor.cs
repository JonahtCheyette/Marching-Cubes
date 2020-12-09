using UnityEngine;
using UnityEditor;

//says to use this editor for the WarpedNoiseHandler Script
[CustomEditor(typeof(WarpedNoiseHandler))]
public class WarpedNoiseHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the WarpedoiseHandler reference
        WarpedNoiseHandler warpedNoiseHandler = (WarpedNoiseHandler)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Generate")) {
            warpedNoiseHandler.GenerateChunks();
        }
    }
}