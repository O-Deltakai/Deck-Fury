using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMapSeed : MonoBehaviour
{

    public void SetMapSeed(string seed)
    {
        GameManager.Instance.SetCustomMapSeed(seed);
    }


}
