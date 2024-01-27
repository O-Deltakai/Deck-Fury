using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes care of instancing item objects such that the player character will be able to use them.
/// As items may have persistent states that extend outside of the battle stage, this class needs to be attached
/// to an object with the PersistentLevelController component so the items are also not destroyed on load.
/// </summary>
[RequireComponent(typeof(PersistentLevelController))]
public class PersistentItemController : MonoBehaviour
{
    private static PersistentItemController _instance;
    public static PersistentItemController Instance => _instance;

    [SerializeField] GameObject itemObjectParent;
    [SerializeField] List<ItemBase> itemObjectsList;
    PlayerDataContainer playerData;


    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        playerData = GetComponent<PersistentLevelController>().PlayerData;
    }

/// <summary>
/// Instantiates the item prefab assosciated with the given ItemSO and adds it to the itemObjectList. <b>This method does not
/// give the item to the player</b>.
/// </summary>
/// <param name="itemSO"></param>
    public ItemBase CreateItem(ItemSO itemSO)
    {
        ItemBase itemPrefab = Instantiate(itemSO.ItemPrefab, itemObjectParent.transform).GetComponent<ItemBase>();
        itemObjectsList.Add(itemPrefab);

        return itemPrefab;
    }

/// <summary>
/// Instantiates the item prefab assosciated with the given ItemSO and adds it to the item list within the player data.
/// </summary>
/// <param name="itemSO"></param>
    public void AddItemToPlayer(ItemSO itemSO)
    {
        ItemBase itemPrefab = Instantiate(itemSO.ItemPrefab, itemObjectParent.transform).GetComponent<ItemBase>();
        itemObjectsList.Add(itemPrefab);
        playerData.AddItem(itemPrefab);
    }



}
