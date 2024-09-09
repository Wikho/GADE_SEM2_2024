using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.Start();
        }

        if (GUILayout.Button("Clear"))
        {
            terrainGenerator.ClearTerrain();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        DrawDefaultInspector();
    }
}
