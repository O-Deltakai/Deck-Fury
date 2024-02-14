using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleSummoner : MonoBehaviour
{
    [SerializeField] GameObject reticlePrefab;

    GameObject _currentReticle;
    public GameObject CurrentReticle { get { return _currentReticle; } }

    void Start()
    {
        _currentReticle = Instantiate(reticlePrefab, Vector3.zero, Quaternion.identity);
        _currentReticle.SetActive(true);
    }

    void OnEnable()
    {
        if(_currentReticle != null)
        {
            _currentReticle.SetActive(true);
        }
    }

    void OnDisable()
    {
        if(_currentReticle != null)
        {
            _currentReticle.SetActive(false);
        }
    }

}
