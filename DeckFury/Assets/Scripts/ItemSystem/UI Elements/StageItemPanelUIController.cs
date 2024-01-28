using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageItemPanelUIController : MonoBehaviour
{
    [SerializeField] GameObject itemListParent;
    [SerializeField] GameObject itemUISlotPrefab;

    void Awake()
    {


    }

    void Start()
    {
        if(PersistentLevelController.Instance)
        {
            InitializeItemList(PersistentLevelController.Instance.PlayerData.GetItemList());
            PlayerItemController playerItemController = GameManager.Instance.player.GetComponent<PlayerItemController>();
            playerItemController.OnAddItemToPlayer += AddItemToPanel;            
        }else
        {
            PlayerItemController playerItemController = GameManager.Instance.player.GetComponent<PlayerItemController>();
            InitializeItemList(playerItemController.ItemList);
            playerItemController.OnAddItemToPlayer += AddItemToPanel;
        }

          
    }


    void InitializeItemList(IReadOnlyList<ItemBase> items)
    {
        foreach (var item in items)
        {
            ItemUISlot prefabInstance = Instantiate(itemUISlotPrefab, itemListParent.transform).GetComponent<ItemUISlot>();
            prefabInstance.SetItem(item);
        }
    }

    void InitializeItemList(List<ItemBase> items)
    {
        foreach (var item in items)
        {
            ItemUISlot prefabInstance = Instantiate(itemUISlotPrefab, itemListParent.transform).GetComponent<ItemUISlot>();
            prefabInstance.SetItem(item);
        }
    }

    public void AddItemToPanel(ItemBase item)
    {
        ItemUISlot prefabInstance = Instantiate(itemUISlotPrefab, itemListParent.transform).GetComponent<ItemUISlot>();
        prefabInstance.SetItem(item);
    }


}
