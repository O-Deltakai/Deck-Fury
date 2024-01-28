using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerItemController : MonoBehaviour
{
    public event Action<ItemBase> OnAddItemToPlayer;

    [SerializeField] List<ItemBase> itemList = new List<ItemBase>();
    public IReadOnlyList<ItemBase> ItemList => itemList;


    void Start()
    {
        if(StageStateController.Instance)
        {
            foreach (var item in StageStateController.Instance.PlayerData.Items)
            {
                GiveItemInstanceToPlayer(item);
            }
        }else
        {
            InitializeItemEffects();
        }

    }

    void OnDestroy()
    {
        DeactivateItems();
    }

    void InitializeItemEffects()
    {
        foreach(var item in itemList)
        {
            item.player = GetComponent<PlayerController>();
            item.PersistentInitialize();
            item.Initialize();
        }
    }

    void DeactivateItems()
    {
        foreach(var item in itemList)
        {
            item.Deactivate();
        }        
    }


/// <summary>
/// This method directly grants an item instance that already exists to the player object on stage - not to be confused with adding an item
/// to the player data of the current run. Items directly added via this method will only stay on the player
/// until the scene is unloaded unless the given item instance is under a DontDestroyOnLoad object.
/// </summary>
/// <param name="item"></param>
    public void GiveItemInstanceToPlayer(ItemBase item)
    {
        itemList.Add(item);
        item.player = GetComponent<PlayerController>();
        item.PersistentInitialize();
        item.Initialize();

        OnAddItemToPlayer?.Invoke(item);
    }

/// <summary>
/// Instantiates a new item instance from the given itemSO and gives it to the player. Items created this way are not persistent and the player
/// will not retain them after the scene is unloaded.
/// </summary>
/// <param name="itemSO"></param>
    public void InstantiateAndGiveItemToPlayer(ItemSO itemSO)
    {
        ItemBase itemPrefab = Instantiate(itemSO.ItemPrefab).GetComponent<ItemBase>();
        GiveItemInstanceToPlayer(itemPrefab);
    }


}
