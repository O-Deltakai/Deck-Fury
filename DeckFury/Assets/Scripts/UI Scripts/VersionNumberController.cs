using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionNumberController : MonoBehaviour
{
    TextMeshProUGUI versionNumber;

    void Awake()
    {
        versionNumber = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        versionNumber.text = "V" + Application.version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
