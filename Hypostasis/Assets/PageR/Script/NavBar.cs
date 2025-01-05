/*
NavBar.cs
This script manages the toggle buttons inorder to changes the pages in the PageR gameObject.
*/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class NavBar : MonoBehaviour {

    /// <summary>
    /// The pageR object this NavBar will be changing
    /// </summary>
    public PageR pageR;


    [Header("Color")]
    //Colors and ColorSettings

    /// <summary>
    /// The update colors or not
    /// </summary>
    public bool updateColors = true;

    /// <summary>
    /// The color off.
    /// </summary>
    public Color colorOff;

    /// <summary>
    /// The color on.
    /// </summary>
    public Color colorOn;

    /// <summary>
    /// The color speed.
    /// </summary>
    public float colorSpeed = 1f;

    [Header("Font")]
    //Font and FontSettings

    /// <summary>
    /// The update font or not
    /// </summary>
    public bool updateFont = true;

    /// <summary>
    /// The font off.
    /// </summary>
    public Font fontOff;

    /// <summary>
    /// The font on.
    /// </summary>
    public Font fontOn;


    /// <summary>
    /// used to calculate the amount the color has changed
    /// </summary>
    private float colorPercent = 0f;

    /// <summary>
    /// The nav buttons...child objects
    /// </summary>
    private GameObject[] NavButtons;

    void Awake()
    {
        //resize the array
        Array.Resize(ref NavButtons,gameObject.transform.childCount);

        //get all the NameButtons!
        for(int i = 0; gameObject.transform.childCount > i ;i++)
        {
            NavButtons[i] = gameObject.transform.GetChild(i).gameObject;
        }

        //Update the name bar when the selectedPage gets updated
        pageR.selectedPageUpdated += UpdateNavBar;

        //Update the NavBar when the pageR becomes Idle
        pageR.startIdle += UpdateNavBar;

        //Update the NavBar now!
        UpdateNavBar();

    }


    void Start()
    {
//        Invoke("disableLayoutGroup",0.1f);
    }

    //diable the LayoutGroup...because the screensize shouldn't change on mobile.
    void disableLayoutGroup()
    {
        GetComponent<HorizontalLayoutGroup>().enabled = false;
    }

    public void UpdateNavBar()
    {
        //for each NavButton
        for(int i = 0; NavButtons.Length > i ;i++)
        {
            //if the selectedPage has the same index as a NavButton...this NavButton's Toggle is On!.
            if ((int)pageR.selectedPage == i)
            {
                NavButtons[i].GetComponent<Toggle>().isOn = true;

                //animate if an animator is attached
                if (NavButtons[i].GetComponent<Animator>())
                {
                    //please make sure you are using the "isOn" bool.
                    NavButtons[i].GetComponent<Animator>().SetBool("isOn",true);
                }
            }
            else
            {
                //set NavButton is off
                NavButtons[i].GetComponent<Toggle>().isOn = false;

                //animate if an animator is attached
                if (NavButtons[i].GetComponent<Animator>())
                {
                    //please make sure you are using the "isOn" bool.
                    NavButtons[i].GetComponent<Animator>().SetBool("isOn",false);
                }
            }
        }

        //Update Color and Fonts...if one of them is true
        if (updateColors || updateFont )
        {
            StopCoroutine("UpdateColorAndFonts");
            StartCoroutine("UpdateColorAndFonts");
        }

    }


    /// <summary>
    /// This method needs to be mapped to the NavButtons. 
    /// </summary>
    /// <param name="i">The index.</param>
    public void NavButtonPress(int i)
    {
        if (pageR.currentState == PageR.states.AUTOSCROLLING)
        {
            return ;
        }

        pageR.ChangePage(i);

    }


    //this Coroutine will change the color and the fonts
    private IEnumerator UpdateColorAndFonts()
    {

        colorPercent = 0f;

        while(colorPercent < 0.95f)
        {
            for(int i = 0; gameObject.transform.childCount > i ;i++)
            {
                
                Toggle tTemp = gameObject.transform.GetChild(i).GetComponent<Toggle>();

                if (tTemp.isOn)
                {

                    if (updateColors)
                    {

                        Color cColor = tTemp.GetComponentInChildren<Image>().color;
                        Color nColor = Color.Lerp(cColor,colorOn,Time.deltaTime * colorSpeed);

                        if (tTemp.targetGraphic != null)
                        {
                            tTemp.targetGraphic.color = nColor;
                        }

                        if (tTemp.graphic != null)
                        {
                            tTemp.graphic.color = nColor;
                        }


                        tTemp.GetComponentInChildren<Text>().color = nColor;
                    }

                    if (updateFont)
                    {
                        tTemp.GetComponentInChildren<Text>().font = fontOn;
                    }

                }
                else
                {
                    if (updateColors)
                    {
                        Color cColor = tTemp.GetComponentInChildren<Image>().color;
                        Color nColor = Color.Lerp(cColor,colorOff,Time.deltaTime * colorSpeed);

                        if (tTemp.targetGraphic != null)
                        {
                            tTemp.targetGraphic.color = nColor;
                        }

                        if (tTemp.graphic != null)
                        {
                            tTemp.graphic.color = nColor;
                        }

                        tTemp.GetComponentInChildren<Text>().color = nColor;
                    }

                    if (updateFont)
                    {
                        tTemp.GetComponentInChildren<Text>().font = fontOff;
                    }

                }

            }

            colorPercent = Mathf.Lerp(colorPercent,1,Time.deltaTime * colorSpeed);

            yield return new WaitForSeconds(0.05f);
//            yield return null;
        }

        //final color update
        if (updateColors)
        {
            for(int i = 0; gameObject.transform.childCount > i ;i++)
            {
                    
                Toggle tTemp = gameObject.transform.GetChild(i).GetComponent<Toggle>();
                if (tTemp.isOn)
                {

                    if (tTemp.targetGraphic != null)
                    {
                        tTemp.targetGraphic.color = colorOn;
                    }

                    if (tTemp.graphic != null)
                    {
                        tTemp.graphic.color = colorOn;
                    }

                    tTemp.GetComponentInChildren<Text>().color = colorOn;

                }
                else
                {

                    if (tTemp.targetGraphic != null)
                    {
                        tTemp.targetGraphic.color = colorOff;
                    }

                    if (tTemp.graphic != null)
                    {
                        tTemp.graphic.color = colorOff;
                    }

                    tTemp.GetComponentInChildren<Text>().color = colorOff;
                }
            }
        }
    }

}
