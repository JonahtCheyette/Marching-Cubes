using UnityEngine;
using UnityEditor;

//says to use this editor for the TestNoiseWarp Script
[CustomEditor(typeof(TestNoiseWarp))]
public class TestNoiseWarpEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the TestNoiseWarp reference
        TestNoiseWarp noiseWarp = (TestNoiseWarp)target;
        DrawDefaultInspector();

        //creates a button that calls Run() when Pressed
        if (GUILayout.Button("Run")) {
            noiseWarp.Run();
        }
    }
}

