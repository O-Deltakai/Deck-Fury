using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggler : MonoBehaviour
{
    [Tooltip("Object to toggle on or off")]
    [SerializeField] GameObject toggledObject;
    
    [Tooltip("List of objects to toggle on or off simultaneously")]
    [SerializeField] List<GameObject> toggledObjectList = new List<GameObject>();


    public void ToggleObject(bool condition)
    {
        if(!toggledObject) { return; }

        toggledObject.SetActive(condition);
    }

   public void ToggleObjectList(bool condition)
    {
        foreach(GameObject toggleObject in toggledObjectList)
        {
            toggledObject.SetActive(condition);
        }
    }


}
