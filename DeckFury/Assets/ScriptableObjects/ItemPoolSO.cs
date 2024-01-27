using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Pool Data", menuName = "New Item Pool", order = 0)]
public class ItemPoolSO : ScriptableObject
{
    [SerializeField] List<ItemSO> _itemPool;
    public IReadOnlyList<ItemSO> ItemPool => _itemPool;    



}
