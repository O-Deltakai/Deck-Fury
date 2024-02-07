using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        playerData = GetComponent<PersistentLevelController>().PlayerData;
    }

    void Start()
    {
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
/// Also returns the instantiated item prefab.
/// </summary>
/// <param name="itemSO"></param>
    public ItemBase AddItemToPlayerData(ItemSO itemSO)
    {
        ItemBase itemPrefab = Instantiate(itemSO.ItemPrefab, itemObjectParent.transform).GetComponent<ItemBase>();
        itemObjectsList.Add(itemPrefab);

        if(GameManager.Instance.player)
        {
            itemPrefab.player = GameManager.Instance.player;
        }


        itemPrefab.PersistentInitialize();

        playerData.AddItem(itemPrefab);
        return itemPrefab;
    }

/// <summary>
/// Destroys the given item instance if it is exists in the itemObjectList.
/// </summary>
/// <param name="item"></param>
/// <returns></returns>
    public bool DestroyItemInstance(ItemBase item)
    {
        if(itemObjectsList.Contains(item))
        {
            itemObjectsList.Remove(item);
            Destroy(item);
            return true;
        }

        return false;
    }

    public bool RemoveItemInstanceFromPlayer(ItemBase item)
    {
        if(playerData.RemoveItem(item))
        {
            DestroyItemInstance(item);
            return true;
        }

        return false;
    }


}
