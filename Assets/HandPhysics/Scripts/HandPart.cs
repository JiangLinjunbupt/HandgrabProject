using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class HandPart : MonoBehaviour
{
   
    public bool IsTouchedObject;
    public bool IsRoot;
    public bool IsHoldingObject;

    public HandPart NextFingerBone;
    public HandPart PrevFingerBone;
    public List<GameObject> CollidedObjects;
    
    void Awake()
    {
        CollidedObjects = new List<GameObject>();
    }
    
    public void TouchObject(GameObject obj)
    {
        IsTouchedObject = true;

        if (PrevFingerBone != null)
        {
            PrevFingerBone.TouchObject(obj);
        }
    }


}
