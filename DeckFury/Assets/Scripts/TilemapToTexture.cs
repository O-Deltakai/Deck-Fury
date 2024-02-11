using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToTexture : MonoBehaviour
{
    [SerializeField] TilemapRenderer _tilemapRenderer;
    public TilemapRenderer tilemapRenderer => _tilemapRenderer;
    [SerializeField] SpriteMask spriteMask;
    MapPreviewGenerator mapPreviewGenerator;

    void Start()
    {
        mapPreviewGenerator = GetComponent<MapPreviewGenerator>();
        if(spriteMask == null)
        {
            Debug.LogError("SpriteMask is not assigned in " + gameObject.name);
            return;
        }
        spriteMask.sprite = mapPreviewGenerator.GeneratePreviewSprite();
    }

    

}
