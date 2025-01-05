/*
ResizeList.cs
Resizes a list so that all Objects fit inside.

*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResizeList : MonoBehaviour {

    /// <summary>
    /// The old height of the item...used to check if there was a change.
    /// </summary>
    private float _itemHeight;

    /// <summary>
    /// The height of the item.
    /// </summary>
    public float itemHeight = 30f;

    /// <summary>
    /// The VerticalLayoutGroup
    /// </summary>
    private VerticalLayoutGroup VLG;

    /// <summary>
    /// The item count.
    /// </summary>
    private int _itemCnt = 0;

    /// <summary>
    /// Gets or sets the item count.
    /// </summary>
    /// <value>The item count.</value>
    public int itemCnt
    {
        get{return _itemCnt;}
        set{

            if (_itemCnt == value)
            {
                return;
            }

            _itemCnt = value;

            resize();
        }
    }

    [Header("the ScrollBars")]

    /// <summary>
    /// The horizontal scroll bar.
    /// </summary>
    public Scrollbar horizontalScrollBar;

    /// <summary>
    /// The vertical scroll bar.
    /// </summary>
    public Scrollbar verticalScrollBar;

	// Use this for initialization
	void Start () 
    {
        //get Reference to the VerticalLayoutGroup
        VLG = GetComponent<VerticalLayoutGroup>();

        //get the Item Count
	    itemCnt = gameObject.transform.childCount;

        //reset the values for the ScrollBars
        Invoke("resetScrollBars",0.01f);

        //set the individual item height
        _itemHeight = itemHeight;
	}

    void FixedUpdate()
    {
        //update if Item Height has changed
        if (_itemHeight != itemHeight)
        {
            resize();
            _itemHeight = itemHeight;
        }
    }

    //reset the values for the scrollbars
    private void resetScrollBars()
    {
        if (horizontalScrollBar != null)
        {
            horizontalScrollBar.value = 1f;
        }

        if (verticalScrollBar != null)
        {
            verticalScrollBar.value = 1f;
        }
    }

	
	//used to resize this gameObject
	private void resize () 
    {

        float fHeight = (itemHeight * itemCnt) + VLG.padding.top + VLG.padding.bottom + (VLG.spacing * (itemCnt-1));

        GetComponent<RectTransform>().sizeDelta = new Vector2(0f, fHeight );

	}
}
