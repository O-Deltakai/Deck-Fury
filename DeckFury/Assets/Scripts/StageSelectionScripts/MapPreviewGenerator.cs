using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreviewGenerator : MonoBehaviour
{
    [SerializeField] Camera mapCamera;
    [field:SerializeField] public RenderTexture MapPreviewRender { get; private set; }

    void Awake()
    {
        mapCamera.enabled = false;
    }    

    public Texture2D GeneratePreviewTexture2D()
    {
        mapCamera.enabled = true;
        mapCamera.targetTexture = MapPreviewRender;

        mapCamera.Render();

        Texture2D previewTexture = RenderTextureToTexture2D(MapPreviewRender);

        mapCamera.enabled = false;

        return previewTexture;
    }

    public Sprite GeneratePreviewSprite()
    {
        Texture2D previewTexture2D = GeneratePreviewTexture2D();

        Sprite previewSprite = Sprite.Create(previewTexture2D, new Rect(0, 0, previewTexture2D.width, previewTexture2D.height), new Vector2(0.5f, 0.5f));

        return previewSprite;
    }

    Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2d.Apply();

        RenderTexture.active = currentActiveRT;
        return texture2d;        
    }

}
