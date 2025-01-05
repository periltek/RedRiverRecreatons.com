PageR
-------------------------------------
[Asset Store Link](http://u3d.as/EMR)  
© 2017 Justin Garza

PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL!
Enjoy! :)

Contact  
-------------------------------------
Questions, suggestions, help needed?  
Contact me at:  
Email: jgarza9788@gmail.com  
Cell: 1-818-251-0647  
Contact Info: [justingarza.net/contact](http://justingarza.net/contact/)
  
Description/Features
-------------------------------------
PageR is a bottom navigation bar template based on MaterialDesign.* Uses unity standard assets.
	* Compatible with all UI Objects.
* Optional NavBar.
* Map 3D Objects to UI positions. 
* MaterialDesign Assets.
	* 256 Material Design Colors.
	* Roboto & Noto Fonts.	* Over 1,500 icons!
Terms of Use
-------------------------------------
You are free to add this asset to any game you’d like
However:  
please put my name in the credits, or in the special thanks section. :)  
please do not re-distribute.  

Table of Contents 
-------------------------------------
1. PageR.cs 
2. NavBar.cs
3. AddTorqueAtStart.cs
4. AlignWithUIObject.cs
5. RecoilAfterScrollBar.cs
6. ResizeList.cs 	   


PageR.cs
-------------------------------------
This script manages the state of the ScrollRect  
(Idle, Scrolling, Recoil, and AutoScrolling);

Below is an image of how PageR.cs will render in the Inspector.  

![Imgur](http://i.imgur.com/nXyESiB.png)

###Variables   (as seen in the inspector)  
  
**PagerID**   
An Id used for saving values to the PlayerPrefs  

**namePages**  
If True this will rename all the pages. example: Page0  

**selectedPage**  
the current selected LevelPage (Updates when the scrollBar states gets reset to Idle)  

**selectedPageFloat**  
the current selected LevelPage as a float  

**saveLastSelectedPage**  
Weather the SelectedPage will be saved in the PlayerPrefs  

**currentState**  
the current state of the scrollBar (Idle, Scrolling, Recoil, and AutoScrolling)  

**recoilSpeed**  
Speed of the scrollBar while Recoiling  

**recoilSensitivity**  
How much you will need to scroll for recoil to move to the next Page.  

**dragHorizontalRatio**  
this is the ratio drag distance X and Y. Only if your drag meets this ratio it will slide to the next page.

**dragHorizontalDistance**  
The drag horizontal distance (in viewport) that you must drag before you can slide to the next page.  
  
  
###Important Methods

**ScrollToLastPosition**  
move the scrollBar to the last position it was saved at.
>saveLastSelectedPage should be True 

**SetState**  
change the current state of the ScrollBar.
>Possible States are Idle, Scrolling, Recoil, and AutoScrolling

**ChangePage**  
allows other scripts to change the Page.  
>See NavBar.cs line 172

**ChangePage_Delta**  
allows other scripts to change the Page, using a pageDelta (+-1) 


NavBar.cs 
-------------------------------------
This script manages the toggle buttons inorder to changes the pages in the PageR gameObject.

Below is an image of how NavBar.cs will render in the Inspector.  

![Imgur](http://i.imgur.com/YUO5eTi.png)

###Variables   (as seen in the inspector)

**pageR**    
The pageR object this NavBar will be changing.

**updateColors**  
Whether the colors should Update or not

**colorOff**  
The color when the NavButton is Off.

**colorOn**  
The color when the NavButton is On.

**colorSpeed**  
The speed of the color change.

**updateFont**  
Whether the font should Update or not

**fontOff**  
The font when the NavButton is Off.

**fontOn**  
The font when the NavButton is Off.


###Important Methods

**NavButtonPress**  
This Method should be mapped to the NavButton so that it will change the page.



AddTorqueAtStart.cs
-------------------------------------
Used to rotate the Cube at start...just for the Demo

Below is an image of how AddTorqueAtStart.cs will render in the Inspector. 

![Imgur](http://i.imgur.com/UNe8mw4.png)

AlignWithUIObject.cs
-------------------------------------
Used to move the cube to align with a UIObject.

Below is an image of how AlignWithUIObject.cs will render in the Inspector. 

![Imgur](http://i.imgur.com/FE5Pyg0.png)

RecoilOnPointerUp.cs
-------------------------------------
Attached to the ScrollBar...so that if the ScrollBar is used the PageR will recoil On pointer up.

ResizeList.cs
-------------------------------------
Resizes a list so that all Objects fit inside.

Below is an image of how ResizeList.cs will render in the Inspector.

![Imgur](http://i.imgur.com/mcd9ozp.png)
