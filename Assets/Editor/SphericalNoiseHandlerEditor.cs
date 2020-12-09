using UnityEngine;
using UnityEditor;

//says to use this editor for the SphericalNoiseHandler Script
[CustomEditor(typeof(SphericalNoiseHandler))]
public class SphericalNoiseHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the SphericalNoiseHandler reference
        SphericalNoiseHandler terrainHandler = (SphericalNoiseHandler)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Generate")) {
            terrainHandler.GenerateChunks();
        }
    }
}
