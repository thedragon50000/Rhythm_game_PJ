using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBaseGameSystem : MonoBehaviour
{
    public virtual void Init(){ }
    public virtual void GameUpdate() { }
    public virtual void Release() { }
}
