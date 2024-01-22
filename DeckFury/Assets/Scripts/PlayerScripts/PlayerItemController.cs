using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerItemController : MonoBehaviour
{
    [SerializeField] List<ItemBase> itemList = new List<ItemBase>();


    void Start()
    {
        InitializeItemEffects();
    }


    void InitializeItemEffects()
    {
        foreach(var item in itemList)
        {
            item.player = GetComponent<PlayerController>();
            item.Initialize();
        }
    }

}
