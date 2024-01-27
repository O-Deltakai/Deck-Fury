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

/// <summary>
/// This method directly grants an item to the player object on stage - not to be confused with adding an item
/// to the player data of the current run. Items directly added will via this method will only stay on the player
/// until the scene is unloaded.
/// </summary>
/// <param name="item"></param>
    public void GiveItemToPlayer(ItemBase item)
    {
        itemList.Add(item);
        item.player = GetComponent<PlayerController>();
        item.Initialize();
    }


}
