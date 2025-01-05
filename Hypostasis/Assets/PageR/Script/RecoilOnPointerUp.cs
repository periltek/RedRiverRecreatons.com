/*
RecoilOnPointerUp.cs
Attached to the ScrollBar...so that if the ScrollBar is used the PageR will recoil when done.
*/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RecoilOnPointerUp : MonoBehaviour , IPointerUpHandler 
{

    /// <summary>
    /// The pageR gameObject
    /// </summary>
    private PageR pageR;

	// Use this for initialization
	void Start () 
    {
	    pageR = gameObject.transform.parent.GetComponent<PageR>();
	}
	
    /// <summary>
    /// Raises the pointer up event.
    /// </summary>
    /// <param name="data">Data.</param>
    public  void OnPointerUp(PointerEventData data)
    {
//        print("OnPointerUp");
        pageR.SetState(PageR.states.RECOIL);

        pageR.thisScrollRect.horizontal = true;
    }
}
