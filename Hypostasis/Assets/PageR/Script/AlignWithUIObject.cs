/*
AlignWithUIObject.cs
Used to move the cube to align with a UIObject.
*/

using UnityEngine;
using System.Collections;

public class AlignWithUIObject : MonoBehaviour {

#region Variables
    private Camera cam ;
    public GameObject UIObject;
    public Vector3 offset;
    public bool AlwaysUpdate = false;
#endregion

#region Methods

    // Use this before initialization
    void Start()
    {
        cam = Camera.main;

        Vector3 v3 = UIObject.transform.position - cam.transform.position;
        Vector3 newPos = cam.ScreenToWorldPoint(v3) + offset;

        gameObject.transform.position = newPos;
    }

    void FixedUpdate()
    {
        if (AlwaysUpdate)
        {

            Vector3 v3 = UIObject.transform.position - cam.transform.position;
            Vector3 newPos = cam.ScreenToWorldPoint(v3) + offset;

            gameObject.transform.position = newPos;
        }
    }
   

#endregion
}
