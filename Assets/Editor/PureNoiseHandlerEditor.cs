using UnityEngine;
using UnityEditor;

//says to use this editor for the PureNoiseHandler Script
[CustomEditor(typeof(PureNoiseHandler))]
public class PureNoiseHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the PureNoiseHandler reference
        PureNoiseHandler pureNoiseHandler = (PureNoiseHandler)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Generate")) {
            pureNoiseHandler.GenerateChunks();
        }
    }
}
