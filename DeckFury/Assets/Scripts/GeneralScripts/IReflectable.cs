using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReflectable
{
    public bool IsReflected { get; set; }
    public virtual void Reflect(GameObject reflector = null){}

}
