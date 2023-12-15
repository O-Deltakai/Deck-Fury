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

        if(condition)
        {
            toggledObject.SetActive(true);
        }else
        {
            toggledObject.SetActive(false);
        }
    }

   public void ToggleObjectList(bool condition)
    {
        if(condition)
        {
            foreach(GameObject toggleObject in toggledObjectList)
            {
                toggledObject.SetActive(true);
            }
        }else
        {
            foreach(GameObject toggleObject in toggledObjectList)
            {
                toggledObject.SetActive(false);
            }
        }
    }


}
