using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Hint Container", menuName = "New Hint Container", order = 0)]
public class HintsContainerSO : ScriptableObject
{

    [TextArea(5, 20)]
    [SerializeField] public string[] HintList;

    public string[] GetHints()
    {
        return HintList;
    }

}
