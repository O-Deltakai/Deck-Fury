using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectionItemPanel : MonoBehaviour
{
    [SerializeField] GameObject itemListParent;
    [SerializeField] GameObject itemUISlotPrefab;
    PlayerDataContainer playerData;

    void Start()
    {
        StartCoroutine(WaitForPlayerData());
    }

    IEnumerator WaitForPlayerData()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        playerData = PersistentLevelController.Instance.PlayerData;

        InitializeItemList(playerData.Items);
        playerData.OnAddItemToPlayer += AddItemToPanel;
    }

    void InitializeItemList(IReadOnlyList<ItemBase> items)
    {
        foreach (var item in items)
        {
            ItemUISlot prefabInstance = Instantiate(itemUISlotPrefab, itemListParent.transform).GetComponent<ItemUISlot>();
            prefabInstance.SetItem(item);
            prefabInstance.FlipDescriptionPanelLocation();
        }
    }

    public void AddItemToPanel(ItemBase item)
    {
        ItemUISlot prefabInstance = Instantiate(itemUISlotPrefab, itemListParent.transform).GetComponent<ItemUISlot>();
        
        prefabInstance.SetItem(item);
        prefabInstance.FlipDescriptionPanelLocation();

    }



}
