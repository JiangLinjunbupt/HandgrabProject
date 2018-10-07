using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum FingersType : byte
{
    Thumb = 1, Index, Middle, Ring, Pinky
}

[Serializable]
public class PositionLimit                                //限制手的移动范围，需要在unit有中测量，暂时用不上
{
    public bool EnableLimits;
    public Vector3 MinPosition = new Vector3(-50, 2, -50);
    public Vector3 MaxPosition = new Vector3(50, 20, 50);
}

public class HandPhysicsController : MonoBehaviour
{
  
    
    public bool EnableControl = true;

    public bool FixedWristRotation; //Don't let to wrist automatically rotate when colliding with other objects
    
    public PositionLimit PositionLimits; //Global position limits for forearm movement

    public HandPart[][] HandParts; //Links to all hand bones
	/*
	
	HandParts[0][0] - Wrist
	HandParts[1][0, 1, 2] - Thumb bones
	HandParts[2][0, 1, 2] - Index bones
	HandParts[3][0, 1, 2] - Middle bones
	HandParts[4][0, 1, 2] - Ring bones
	HandParts[5][0, 1, 2] - Pinky bones	
	*/

    
    public bool ObjectAttached; //Is any rigidbody object attached to hand?
    private GameObject _objectToAttach;

    void Awake()
    {
        InitHand();
    }

    void InitHand() //Initialize ang configure bones of this hand
    {
        //Initalize HandPart array
        #region InitArray

        HandParts = new HandPart[6][];

        HandParts[0] = new HandPart[1];
        for (int i = 1; i < HandParts.Length; i++)
        {
            HandParts[i] = new HandPart[3];
        }
        #endregion

        //Find all hand parts by name of this transform
        #region HandPartsFinding

        HandParts[0][0] = transform.Find("Wrist").gameObject.AddComponent<HandPart>();
        for (int i = 1; i < HandParts.Length; i++)
        {
            FingersType fingerType = (FingersType)i;
            HandParts[i][0] = HandParts[0][0].transform.Find(fingerType.ToString() + "0").gameObject.AddComponent<HandPart>();
            HandParts[i][0].IsRoot = true;

            for (int j = 1; j < HandParts[i].Length; j++)
            {
                HandParts[i][j] = HandParts[i][j - 1].transform.Find(fingerType.ToString() + j).gameObject.AddComponent<HandPart>();

                HandParts[i][j].PrevFingerBone = HandParts[i][j - 1];
                HandParts[i][j - 1].NextFingerBone = HandParts[i][j];
            }
        }
        #endregion

        //Create collision detectors for all fingers
        #region CollisionDetectors                                                   //  ??????????????????????????????????????????????????

        for (int i = 1; i < HandParts.Length; i++)
        {
            FingersType fingerType = (FingersType)i;

            for (int j = 0; j < HandParts[i].Length; j++)
            {
                if (j == 0)
                    continue;
                GameObject collisionDetector = (GameObject)Instantiate(HandParts[i][j].gameObject);
                Destroy(collisionDetector.GetComponent<HandPart>());

                var children = new List<GameObject>();
                foreach (Transform child in collisionDetector.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                collisionDetector.name = fingerType.ToString() + j + "_colDetector";
                collisionDetector.transform.parent = HandParts[i][j].transform;
                collisionDetector.transform.localPosition = Vector3.zero;
                collisionDetector.transform.localRotation = Quaternion.identity;
                collisionDetector.transform.localScale = HandParts[i][j].transform.localScale;

                CapsuleCollider collider = collisionDetector.GetComponent<CapsuleCollider>();
                collider.isTrigger = true;

                collisionDetector.AddComponent<FingerCollisionDetector>();
                collisionDetector.GetComponent<FingerCollisionDetector>().ThisHandPart = HandParts[i][j];
                //HandParts[i][j].gameObject.GetComponent<FingerCollisionDetector>().ThisHandPart = HandParts[i][j];
            }

            }
        
        #endregion
        //Ignore collisions between all hand parts
        #region IgnoreCollisions

        foreach (var handPartParent in HandParts)
        {
            foreach (var handPart in handPartParent)
            {
                for (int i = 0; i < HandParts.Length; i++)
                {
                    for (int j = 0; j < HandParts[i].Length; j++)
                    {
                        if (HandParts[i][j] != handPart)
                            //Physics.IgnoreCollision(HandParts[i][j].collider, handPart.collider);
                            Physics.IgnoreCollision(HandParts[i][j].GetComponent<Collider>(), handPart.GetComponent<Collider>());
                    }

                }

            }
        }
        #endregion

    }


    void FixedUpdate()
    {

        
        //Grab object with non-kinematic rigidbody component by checking fingers collisions
        #region ObjectGrabbing

        if (!ObjectAttached)
        {
            if (CheckIfCanAttach())
            {
                if (_objectToAttach != null)
                {
                    AttachObject(_objectToAttach.GetComponent<Rigidbody>());
                }
            }
        }
        else
        {
            
            if (!HandParts[1][2].IsTouchedObject)
            {
                DetachObject();
            }
        }

        #endregion
    }

    bool CheckIfCanAttach()
    {
        bool thumbIsReady = false;
        List<GameObject> thumbCollidedObjects = new List<GameObject>();
        for (int i = 0; i < HandParts[1].Length; i++)
        {
            if (HandParts[1][i].IsTouchedObject && HandParts[1][2].CollidedObjects.Count!=0)
            {
                HandParts[1][i].IsHoldingObject = true;
                thumbIsReady = true;
                Debug.Log("Thumb Touch");
            }
           
            thumbCollidedObjects.AddRange(HandParts[1][i].CollidedObjects);
        }

        if (!thumbIsReady)
        {
            foreach (var thumb in HandParts[1])
            {
                thumb.IsHoldingObject = false;
            }
            return false;
        }

        
        for (int i = 2; i < HandParts.Length; i++)
        {
            for (int j = 1; j < HandParts[i].Length; j++)
            {
                if (HandParts[i][j].IsTouchedObject && HandParts[1][2].CollidedObjects.Count != 0)
                {
                    foreach (var collidedObject in HandParts[i][j].CollidedObjects)
                    {
                        if (thumbCollidedObjects.Contains(collidedObject))
                        {
                            HandParts[i][j].IsHoldingObject = true;
                            _objectToAttach = collidedObject;
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    void AttachObject(Rigidbody rb)
    {
        foreach (var handPartRoots in HandParts)
        {
            foreach (var handPart in handPartRoots)
            {
                Physics.IgnoreCollision(handPart.GetComponent<Collider>(), rb.GetComponent<Collider>());
            }
        }


        Debug.Log("Add FixedJoint step1");
        HandParts[0][0].gameObject.AddComponent<FixedJoint>();
        Debug.Log("Add FixedJoint step2");
        HandParts[0][0].gameObject.GetComponent<FixedJoint>().connectedBody = rb;
        Debug.Log("Add FixedJoint step3");
        HandParts[0][0].gameObject.GetComponent<FixedJoint>().enableCollision = true;
        Debug.Log("Add FixedJoint step4");
        
        ObjectAttached = true;
    }

    void DetachObject()
    {
        if (_objectToAttach != null)
        {
            foreach (var handPartRoots in HandParts)
            {
                foreach (var handPart in handPartRoots)
                {
                    Physics.IgnoreCollision(handPart.GetComponent<Collider>(), _objectToAttach.GetComponent<Collider>(), false);
                }
            }
        }
       
        Destroy(HandParts[0][0].GetComponent<FixedJoint>());
        ObjectAttached = false;
        _objectToAttach = null;
    }
    
}
