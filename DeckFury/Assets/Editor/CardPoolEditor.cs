using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardPoolSO))]
public class CardPoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Sort Cards By Tier"))
        {
            CardPoolSO cardPool = (CardPoolSO)target;
            cardPool.SortCardsByTier();
        }


    }


}
