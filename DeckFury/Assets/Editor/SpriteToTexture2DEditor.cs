using UnityEngine;
using UnityEditor;

public class SpriteToTexture2DEditor : EditorWindow
{
    private Sprite spriteToConvert;

    [MenuItem("Tools/Convert Sprite to Texture2D")]
    public static void ShowWindow()
    {
        GetWindow<SpriteToTexture2DEditor>("Sprite to Texture2D");
    }

    void OnGUI()
    {
        GUILayout.Label("Convert Sprite to Texture2D", EditorStyles.boldLabel);

        spriteToConvert = EditorGUILayout.ObjectField("Sprite", spriteToConvert, typeof(Sprite), false) as Sprite;

        if (GUILayout.Button("Convert"))
        {
            if (spriteToConvert != null)
            {
                ConvertSpriteToTexture2D(spriteToConvert);
            }
            else
            {
                Debug.LogError("No sprite selected for conversion.");
            }
        }
    }

    private void ConvertSpriteToTexture2D(Sprite sprite)
    {
        if (sprite == null) return;

        Texture2D newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, 
                                                  (int)sprite.textureRect.y, 
                                                  (int)sprite.textureRect.width, 
                                                  (int)sprite.textureRect.height);
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        // Save the new texture as an asset
        byte[] bytes = newTexture.EncodeToPNG();
        var path = EditorUtility.SaveFilePanelInProject("Save Texture2D", sprite.name + "_Texture", "png", "Please enter a file name to save the texture to");
        if (path.Length != 0)
        {
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureType = TextureImporterType.Default;

            // Update the importer settings if needed
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}