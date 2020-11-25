using UnityEngine;
using UnityEditor;

//says to use this editor for the NoiseDensityChunkHandler Script
[CustomEditor(typeof(PureNoiseHandler))]
public class PureNoiseHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the chunkGenerator reference
        PureNoiseHandler pureNoiseHandler = (PureNoiseHandler)target;
        DrawDefaultInspector();

        //generates a chunk
        if (GUILayout.Button("Generate")) {
            pureNoiseHandler.GenerateChunks();
        }
    }
}
