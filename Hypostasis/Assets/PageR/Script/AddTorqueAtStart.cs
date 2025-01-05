/*
AddTorqueAtStart.cs
Used to rotate the Cube at start...just for the Demo
*/

using UnityEngine;
using System.Collections;

public class AddTorqueAtStart : MonoBehaviour {

    public Vector3 torque = new Vector3(2.5f,10f,5f);

	// Use this for initialization
	void Start () 
    {
        GetComponent<Rigidbody>().AddTorque(torque);
	}

}
