using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCountPreviewController : MonoBehaviour
{
    public SpawnTableSO currentSpawnTable;

    [SerializeField] GameObject previewEnemyPrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetPreviewBySpawnTable(SpawnTableSO spawnTable)
    {
        if(currentSpawnTable == spawnTable)
        {
            return;
        }

        if(currentSpawnTable != null)
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        currentSpawnTable = spawnTable;

        List<NPCSpawnData> spawnData = spawnTable.GetAllNPCSpawnData();
        foreach(NPCSpawnData data in spawnData)
        {
            GameObject previewEnemySlot = Instantiate(previewEnemyPrefab, transform);
            previewEnemySlot.GetComponent<Image>().sprite = data.NPCPrefab.GetComponent<EntityWrapper>().stageEntity.GetComponent<NPC>().EnemyData.PreviewSprite;
            TextMeshProUGUI enemyCountText = previewEnemySlot.GetComponentInChildren<TextMeshProUGUI>();
            enemyCountText.text = data.SpawnCount.ToString() + "x";
            
        }

    }


}
