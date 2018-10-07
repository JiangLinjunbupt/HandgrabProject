using UnityEngine;
using System.Collections;

public class HandPhysicsControllerInput : MonoBehaviour
{
    private HandPhysicsController _handController;

	void Start ()
	{
	    _handController = GetComponent<HandPhysicsController>();
	}

    void FixedUpdate()
    {
        if (Camera.main != null)
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _handController.HandParts[0][0].transform.position + new Vector3(0, 7, 6), Time.deltaTime * 15);
    }

    void Update () 
    {
        //Enable or disable control
	    if (Input.GetKeyDown(KeyCode.C))
	        _handController.EnableControl = !_handController.EnableControl;
    }
}
