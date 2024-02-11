using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TilemapRenderer))]
public class TilemapSnapshotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TilemapRenderer tilemapRenderer = (TilemapRenderer)target;

        if (GUILayout.Button("Save Tilemap Snapshot"))
        {
            TakeSnapshot(tilemapRenderer);
        }
    }

    void TakeSnapshot(TilemapRenderer tilemapRenderer)
    {
        // Define the RenderTexture and Texture2D sizes
        int width = 1920; // Adjust these values to match your tilemap size
        int height = 1080;

        // Create a RenderTexture
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        renderTexture.Create();

        // Create a temporary Camera
        GameObject tempCameraGO = new GameObject("TempCamera");
        Camera tempCamera = tempCameraGO.AddComponent<Camera>();
        tempCamera.targetTexture = renderTexture;
        tempCamera.transform.position = tilemapRenderer.transform.position + Vector3.back * 10; // Adjust camera position as needed

        tempCamera.cullingMask = 1 << LayerMask.NameToLayer(LayerNames.MapLayoutWalls.ToString());

        // Render the tilemap
        tempCamera.Render();

        // Transfer image from RenderTexture to Texture2D
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        RenderTexture.active = null; // Reset active RenderTexture

        // Save Texture2D as PNG
        byte[] bytes = texture.EncodeToPNG();
        string path = "Assets/TilemapSnapshots/" + tilemapRenderer.name + "_snapshot.png";
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        // Create a sprite from the texture
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.SaveAndReimport();

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        // Cleanup
        DestroyImmediate(tempCameraGO); // Use DestroyImmediate in the editor
        DestroyImmediate(renderTexture);

        Debug.Log("Snapshot saved as " + path);
    }
}