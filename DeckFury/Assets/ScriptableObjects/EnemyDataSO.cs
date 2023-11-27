using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy Data", menuName = "New Enemy Data", order = 0)]
public class EnemyDataSO : ScriptableObject
{

    [SerializeField] string _enemyName;
    public string EnemyName{get { return _enemyName; }}

[Header("Combat Stats")]
    [SerializeField] int _maxHP;
    public int MaxHP{get{ return _maxHP; }}

    [SerializeField] int _shieldHP;
    public int ShieldHP{get{ return _shieldHP; }}



    [SerializeField] List<AttackElement> _weaknesses;
    public List<AttackElement> Weaknesses{get{ return _weaknesses; }}

    [SerializeField] List<AttackElement> _resistances;
    public List<AttackElement> Resistances{get{ return _resistances; }}




}
