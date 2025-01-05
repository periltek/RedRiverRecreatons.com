/*
PageR.cs
This script manages the state of the ScrollRect (IDLE,SCROLLING,RECOIL,AUTOSCROLLING);
*/


#if UNITY_EDITOR
    using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
[ExecuteInEditMode]
public class PageR : MonoBehaviour , IPointerUpHandler , IPointerDownHandler , IDragHandler 
{

#region Variables

    /// <summary>
    /// An Id used for saving values to the PlayerPrefs
    /// </summary>
    public string PagerID = "PageR001_";

    /// <summary>
    /// If True this will rename all the pages. example: Page0
    /// </summary>
    public bool namePages = true;

    /// <summary>
    /// the current selected LevelPage (Updates when the scrollBar states gets reset to Idle)
    /// </summary>
    public float selectedPage;

    /// <summary>
    /// the current selected LevelPage as a float
    /// </summary>
    public float selectedPageFloat;

    /// <summary>
    /// Weather the SelectedPage will be saved in the PlayerPrefs
    /// </summary>
    public bool saveLastSelectedPage = true;
    public int DefaultPage = 0;

    /// <summary>
    /// _SP/SP are used to set and update the selectedPage/selectedPageFloat
    /// </summary>
    private int _SP;
    private int SP
    {
        get {return _SP;}

        set {
                _SP = value;
                UpdateSP(value);

                if (saveLastSelectedPage)
                {
                    PlayerPrefs.SetFloat(PagerID + "selectedPage",selectedPage);
                }
            }
    }



    [Header("Scroll Settings")]


    /// <summary>
    /// the current state of the scrollBar (IDLE,SCROLLING,RECOIL,AUTOSCROLLING)
    /// </summary>
    public states currentState;

    /// <summary>
    /// possible states for the scrollBar
    /// </summary>
    public enum states
    {
        IDLE,SCROLLING,RECOIL,AUTOSCROLLING
    }

    [Range(0.1f, 10.0f)]
    /// <summary>
    /// Speed of the scrollBar while Recoiling
    /// </summary>
    public float recoilSpeed = 1f;

    /// <summary>
    /// How much you will need to scroll for recoil to move to the next Page
    /// </summary>
    public float recoilSensitivity = 5;

    /// <summary>
    /// this is the ratio drag distance X and Y. 
    /// Only if your drag meets this ratio it will slide to the next page.
    /// </summary>
    [Range(0.1f, 1f)]
    public float dragHorizontalRatio = 0.333f;

    /// <summary>
    /// The drag horizontal distance (in viewport) that you must drag before you can slide to the next page.
    /// </summary>
    public float dragHorizontalDistance = 0.01f;

    [HideInInspector]
    /// <summary>
    /// will scroll to this position during auto-scroll State.
    /// </summary>
    public float scrollToPosition = -1f;


    [HideInInspector]
    /// <summary>
    /// The GameObject that contains the content of the ScrollRect
    /// </summary>
    public GameObject contentObject;

    [HideInInspector]
    /// <summary>
    /// the scrollBar we will be using (Horizontal/Vertical)
    /// </summary>
    public Scrollbar thisScrollBar;

    [HideInInspector]
    public ScrollRect thisScrollRect; 

    [HideInInspector]
    /// <summary>
    /// the Number of LevelPages
    /// </summary>
    public int pageCount;

    /// <summary>
    /// The dictionary that maps the scrollBar states to methods to execute.
    /// </summary>
    public Dictionary<states, Action> dictionary = new Dictionary<states, Action>();


    /*
    Generic Delegate
    --Delegates store one or more methods, and when the delegate is called all methods it was storing will be called upon.
    learn more about delegates at https://msdn.microsoft.com/en-us/library/ms173172.aspx
    */
    public delegate void genericDelegate();

    //Idle Delegates
    public genericDelegate startIdle;
    public genericDelegate duringIdle;
    public genericDelegate endIdle;

    //Scorlling Delegates
    public genericDelegate startScrolling;
    public genericDelegate duringScrolling;
    public genericDelegate endScrolling;

    //Autoscrolling Delegates
    public genericDelegate startAutoscrolling;
    public genericDelegate duringAutoscrolling;
    public genericDelegate endAutoscrolling;

    //Recoil Delegates
    public genericDelegate startRecoil;
    public genericDelegate duringRecoil;
    public genericDelegate endRecoil;

    //SelectedPageUpdate Delegate
    public genericDelegate selectedPageUpdated;


#endregion

#region MonoMethods

    void Awake()
    {

        //get the ScrollRect
        thisScrollRect = gameObject.GetComponent<ScrollRect>();
        thisScrollRect.inertia = false;

        //get a reference to the scrollbar
        thisScrollBar = thisScrollRect.horizontalScrollbar;

        //disable vertical and enable horizontal
        thisScrollRect.vertical = false;
        thisScrollRect.horizontal = true;

        //set startCorner for GridLayoutGroup;
        contentObject.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;

    }

    void Start()
    {

        //find the contentObject, and get the pageCount
        contentObject = gameObject.transform.Find("Viewport/Content").gameObject;
        pageCount = contentObject.transform.childCount;

        //resize the contentObject
        ResizeContent(pageCount,contentObject);

        //can't have zero
        if (pageCount == 0)
        {
            Debug.LogError("there are no LevelPages, please rebuildLevelPages");
            return;
        }


        //the current state should be Idle
        currentState = states.IDLE;

        //map the states to the methods
        dictionary.Add(states.IDLE,Idle);
        dictionary.Add(states.SCROLLING,Scrolling);
        dictionary.Add(states.RECOIL,Recoil);
        dictionary.Add(states.AUTOSCROLLING,Autoscrolling);

        //scroll to the last position
        Invoke("ScrollToLastPosition",0.01f);

    }

    void Update()
    {
        //lock the horizontal scrolling if MouseButtonDown, unlock will occur if user drags in the correct direction.
        if (Input.GetMouseButtonDown(0))
        {
            thisScrollRect.horizontal =  false;
            pointerPosDown = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        //unlock if MouseButtonUp
        if (Input.GetMouseButtonUp(0))
        {
            thisScrollRect.horizontal =  true;
        }


#if UNITY_EDITOR
        //the following will only execute while the editor is not playing
        if (EditorApplication.isPlaying)
        {
            return;
        }

        //rename Pages
        if (namePages)
        {
            for(int i = 0;contentObject.transform.childCount > i;i++)
            {
                contentObject.transform.GetChild(i).gameObject.name = "Page" + i.ToString();
            }

            namePages = false;
        }

        //get the contentObject and PageNum
        contentObject = gameObject.transform.Find("Viewport/Content").gameObject;
        pageCount = contentObject.transform.childCount;

        //resize the contentObject
        ResizeContent(pageCount,contentObject);
#endif
    }

    void FixedUpdate()
    {
        //call the method that is mapped to the current state
        dictionary[currentState].Invoke();

        //if the current state is Idel, and the delegate is not null, then execute the during delegate
        if (currentState == states.IDLE)
        {
            if (duringIdle != null)
            {
                duringIdle();
            }
        } //if the current state is Scrolling, and the delegate is not null, then execute the during delegate
        else if (currentState == states.SCROLLING)
        {
            if (duringScrolling != null)
            {
                duringScrolling();
            }
        }
        else if (currentState == states.AUTOSCROLLING)
        {
            if (duringAutoscrolling != null)
            {
                duringAutoscrolling();
            }
          
        } //if the current state is Recoil, and the delegate is not null, then execute the during delegate
        else if (currentState == states.RECOIL)
        {
            if (duringRecoil != null)
            {
                duringRecoil();
            }
        }

    }

#endregion

#region Methods


    //move the scrollBar to the last position it was saved at
    public void ScrollToLastPosition()
    {
        if (saveLastSelectedPage)
        {
            SP = (PlayerPrefs.HasKey(PagerID + "selectedPage"))?  (int)(PlayerPrefs.GetFloat(PagerID + "selectedPage")) : DefaultPage;
            SP = (SP == 0)?1:SP;

        }
        else
        {
            SP = DefaultPage;
        }

        ChangePage((int)SP);

        thisScrollBar.value = scrollToPosition;
    }

    //change the current state of the ScrollBar
    public void SetState(states NextState)
    {
        //can't set to the same state 
        if (currentState == NextState)
        {
            return;
        }

        //do not allow to change from RECOIL to AUTOSCROLLING
        if (currentState == states.RECOIL
            && NextState == states.AUTOSCROLLING
            )
        {
            return;
        }

        if (currentState == states.IDLE)
        {
            if (endIdle != null)
            {
                endIdle();
            }


        }
        else if (currentState == states.SCROLLING)
        {
            if (endScrolling != null)
            {
                endScrolling();
            }


        }
        else if (currentState == states.AUTOSCROLLING)
        {
            if (endAutoscrolling != null)
            {
                endAutoscrolling();
            }
        }
        else if (currentState == states.RECOIL)
        {
            if (endRecoil != null)
            {
                endRecoil();
            }


        }

        print("New State: " + NextState.ToString());
        currentState = NextState;

        if (currentState == states.IDLE)
        {
            if (startIdle != null)
            {
                startIdle();
            }

        }
        else if (currentState == states.SCROLLING)
        {
            if (startScrolling != null)
            {
                startScrolling();
            }

        }
        else if (currentState == states.AUTOSCROLLING)
        {
            if (startAutoscrolling != null)
            {
                startAutoscrolling();
            }

        }
        else if (currentState == states.RECOIL)
        {

            if (startRecoil != null)
            {
                startRecoil();
            }

        }

    }

    //resize the contentObject
    private void ResizeContent(int PC, GameObject CO)
    {
        // -1 for page count.
        PC -= 1;

        //get the Rect of this GameObject
        Rect r = GetComponent<RectTransform>().rect;

        //set the cell size...the size for each Page
        CO.GetComponent<GridLayoutGroup>().cellSize = new Vector2(r.width ,r.height);

        //set the size for to store all the pages
        CO.GetComponent<RectTransform>().sizeDelta = new Vector2( (r.width + 1) * PC,r.height * 0.99f );

#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            CO.GetComponent<RectTransform>().anchoredPosition = new Vector2((r.width/-2f)/PC,r.height * 0f);
//            CO.transform.localPosition = new Vector3( 0f,0f ,0f); 
        }
        else
        {
            CO.transform.localPosition = new Vector3( 0f,0f ,0f); 
        }
#else
        CO.transform.localPosition = new Vector3( 0f,0f ,0f); 
#endif


    } 

    //update the selectedPage
    private void UpdateSP()
    {
        selectedPageFloat = (thisScrollBar.value / (1f/(pageCount-1f)));

        if (selectedPage != Mathf.RoundToInt(selectedPageFloat))
        {
            selectedPage = Mathf.RoundToInt(selectedPageFloat);

            if (selectedPageUpdated != null)
            {
                selectedPageUpdated();
            }
        }

    }

    //update the selectedPage
    private void UpdateSP(int value)
    {
        selectedPageFloat = (float)value;

        if (selectedPage != Mathf.RoundToInt(selectedPageFloat))
        {
            selectedPage = Mathf.RoundToInt(selectedPageFloat);

            if (selectedPageUpdated != null)
            {
                selectedPageUpdated();
            }
        }
    }


    //allows other scripts to change the Page.
    public void ChangePage(int pageNum)
    {
        scrollToPosition = (float)pageNum/(float)(pageCount-1);

        SetState(states.AUTOSCROLLING);
    }

    //allows other scripts to change the Page, using a pageDelta (+-1)
    public void ChangePage_Delta(int pageDelta)
    {
        int pageNum = ((int)selectedPage + pageDelta);

        scrollToPosition = (float)pageNum/(float)(pageCount-1);

        SetState(states.AUTOSCROLLING);
    }

#endregion
#region StateMethods

    //the following methods are mapped to the scrollBar states

    //this method is used while the scrollBar is Idel
    public void Idle()
    {
        //nothing
    }

    //this method is used while the scrollBar is Scrolling
    public void Scrolling()
    {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_STANDALONE || UNITY_WEBGL
        //if the mousebutton goes up...go to Recoil
        if (Input.GetMouseButtonUp(0)) 
        {
            SetState(states.RECOIL);
        }

        //if you stop scrolling...go to Recoil
        if ( 
            Input.mouseScrollDelta == Vector2.zero
            && !Input.GetMouseButton(0)
            )
        {
            SetState(states.RECOIL);
        }
#endif

#if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_TIZEN
        //if there is no touches...go to Recoil
        if (Input.touchCount == 0)
        {
            SetState(states.RECOIL);
        }
#endif

        UpdateSP();
    }

    //this method is used while the scrollBar is Recoil
    public void Recoil()
    {


        //Loop to find the closest LevelPage
        
        float lowestValue = 1000f;
        int i_low = 0;

        for (int i =0 ;i < pageCount;i++)
        {

            float value  = Mathf.Abs((float)i/(float)(pageCount-1) - thisScrollBar.value );

            if (i != SP)
            {
                value = value/recoilSensitivity;
            }

            if ( 
                value < lowestValue 
                )
            {
                lowestValue = value;
                i_low = i;
            }
        }


        //lerp to the new scroll value
        thisScrollBar.value = Mathf.Lerp(thisScrollBar.value, (float)i_low/(float)(pageCount-1) ,Time.deltaTime * recoilSpeed);

        //Update the selectedPageFloat
        UpdateSP();

        //if the values are close enough...go to Idel
        if (Mathf.Abs(thisScrollBar.value - (float)i_low/(float)(pageCount-1)) < 0.001f)
        {
            SP = i_low;
            thisScrollBar.value = (float)i_low/(float)(pageCount-1);

            SetState(states.IDLE);
        }

    }

    //this method is used while the scrollBar is Autoscrolling
    public void Autoscrolling()
    {
        thisScrollBar.value = Mathf.Lerp(thisScrollBar.value,scrollToPosition,Time.deltaTime * recoilSpeed);

        if ( Mathf.Abs(thisScrollBar.value - scrollToPosition) < 0.001f)
        {
            SetState(states.RECOIL);
        }

        //Update the selectedPageFloat
        UpdateSP();

    }

#endregion

#region Events
//these effects exist due to inheriting IDragHandler, IPointerDownHandler , IPointerUpHandler

    private Vector3 pointerPosDown;

    /// <summary>
    /// Raises the drag event.
    /// </summary>
    /// <param name="data">Data.</param>
    public  void OnDrag(PointerEventData data) 
    {
//        print("OnDrag");
        Vector3 DP = Camera.main.ScreenToViewportPoint(data.position);

        //see if drag meets the criteria for scrolling...and scroll.
        if (
            Mathf.Abs(DP.y - pointerPosDown.y) < (Mathf.Abs(DP.x - pointerPosDown.x) * dragHorizontalRatio)
            && Mathf.Abs(DP.x - pointerPosDown.x) > dragHorizontalDistance
            )
        {
            SetState(states.SCROLLING);
            thisScrollRect.horizontal =  true;
        }
    }

    /// <summary>
    /// Raises the pointer up event.
    /// </summary>
    /// <param name="data">Data.</param>
    public  void OnPointerUp(PointerEventData data)
    {
//        print("OnPointerUp");
        SetState(states.RECOIL);

        thisScrollRect.horizontal = true;
    }

    /// <summary>
    /// Raises the pointer down event.
    /// </summary>
    /// <param name="data">Data.</param>
    public  void OnPointerDown (PointerEventData data) 
    {
//        print("OnPointerDown");
        pointerPosDown = Camera.main.ScreenToViewportPoint(data.position);
        thisScrollRect.horizontal =  false;
    }

 

#endregion

}


