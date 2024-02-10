using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapPreviewGenerator))]
public class CameraRenderToMask : MonoBehaviour
{
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
