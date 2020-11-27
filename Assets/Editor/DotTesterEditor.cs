using UnityEngine;
using UnityEditor;

//says to use this editor for the dotTester Script

[CustomEditor(typeof(DotTester))]
public class DotTesterEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the chunkGenerator reference
        DotTester dotTester = (DotTester)target;
        DrawDefaultInspector();

        //generates a matrix of dots
        if (GUILayout.Button("Test With Dots")) {
            dotTester.TestWithDots();
        }
    }
}
//Dot tester script currently not active