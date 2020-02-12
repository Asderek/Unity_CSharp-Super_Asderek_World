using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a basic abstraction of a scrolled Menu
/// </summary>
[System.Serializable]
public abstract class ScrollMenu : AbstractMenu
{

    [Tooltip ("Number of elements shown")]
    public int elementsToShow;

    protected List<Area> elementsArea;
    [HideInInspector]
    public LimitedIndex topMenuIndex;           // current selected menu element
    [HideInInspector]
    public LimitedIndex backgroundIndex;        // current background element
    [HideInInspector]
    public int offsetPosition;
    private bool isMoving;


    /// <summary>
    /// Draw the background element.
    /// </summary>
    /// <param name="rect">The rect to draw</param>
    /// <param name="index">The index to acess background elements</param>
    public abstract void DrawBackgroundElement(Rect rect, int index);

    /// <summary>
    /// Draws the fragment background element.
    /// </summary>
    /// <param name="percentHeight">Percent Height of the menu element.</param>
    /// <param name="goingUp">if true the background element is going up.</param>
    /// <param name="rect">The rect to draw</param>
    /// <param name="elementIndex">The index to acess background elements</param>
    public abstract void DrawFragmentBackgroundElement(float percentHeight, bool goingUp, Rect rect, int elementIndex);

    /// <summary>
    /// Function that will be called to draw each visible element on screen
    /// The occupied area calculation will be made in another place to separate the logic to show one menu element of the rest
    /// </summary>
    /// <param name="percentHeight">Percent Height of the menu element.</param>
    /// <param name="drawUp">if true the background element is going up.</param>
    /// <param name="rect">The rect to draw.</param>
    /// <param name="elementIndex">The index to acess background elements.</param>
    /// <param name="isActive">True if this is the active menu.</param>
    public abstract void DrawFragmentElement(float percentHeight, bool drawUp, Rect rect, int elementIndex, bool isActive);

    /// <summary>
    /// Menus should read in Updates same Input to change the element selected, page by page
    /// </summary>
    /// <returns>Should return:
    ///     <para>higher than zero to increment - Increment Index - Element selected on Screen down</para>
    ///     <para>lower than zero to decrement - Decrement Index - Element selected on Screen up</para>
    ///     <para>equal to zero to keep the value</para>
    /// </returns>
    public abstract float GetPageScroll();

    /// <summary>
    /// Function to initialize some internal properties. Should be called in Start()
    /// </summary>
    /// <param name="menu">Index to control the menu index</param>
    /// <param name="background">Index to control the background index</param>
    public void Init(LimitedIndex menu, LimitedIndex background, bool verticalSeparation=true)
    {
        base.Init(menu);
        this.verticalSeparation = verticalSeparation;
        elementsArea = new List<Area>();
        topMenuIndex = new CircularIndex(menu.Max());
        backgroundIndex =background;
        offsetPosition = 0;
        isMoving = false;

        UpdateNumberOfElements();
    }

    /// <summary>
    /// Function to make to control of movement and resizing operations also to change the selected element.
    /// Should be called in Update() of Monobehavior always that menu will be displayed
    /// </summary>
    public override void Update()
    {        
        for (int i = 0; i < elementsArea.Count; i++)
        {
            elementsArea[i].Update();
        }

        if (isMoving)
            return;

        if (offsetPosition > 0) {
            Move(true);
            offsetPosition--;
            return;
        } else if (offsetPosition < 0)
        {
            Move(false);
            offsetPosition++;
            return;
        }
        
        float axis = GetPageScroll();
        if ( axis > 0)
        {
            offsetPosition = elementsToShow;
            return;
        }
        else if (axis < 0)
        {
            offsetPosition = -elementsToShow;
            return;
        }

        base.Update();

        if (elementsToShow < menuIndex.Max())
        {

            if (GetAxis() > 0)
            {
                if (CircularIndex.UpDiff(topMenuIndex,menuIndex) >= elementsToShow)
                {
                    offsetPosition = 1;
                    menuIndex--;    // cancel menuIndex++ of base.Update
                    ChangeSelection();
                }
            }
            else if (GetAxis() < 0)
            {
                if (CircularIndex.CircularDiff(topMenuIndex, menuIndex) < 0)
                {
                    offsetPosition = -1;
                    menuIndex++;    // cancel menuIndex-- of base.Update
                    ChangeSelection();
                }
            }
        }
    }

    /// <summary>
    /// A function that will be called when offsetPosition is diferrent of zero. Will perform the movement of the menu.
    /// </summary>
    /// <param name="up">if set to <c>true</c> [up].</param>
    protected void Move(bool up)
    {
        isMoving = true;

        if (up)
        {
            Area area;
            Area.Position newPosition;
            if (verticalSeparation)
            {
                area = new Area(0.5f, 1, 1, 1 / ((float)elementsToShow), menuArea.growStep, menuArea.moveStep);
                newPosition = new Area.Position(0.5f, 0); 
            }
            else
            {
                area = new Area(1f, 0.5f, 1 / ((float)elementsToShow), 1, menuArea.growStep, menuArea.moveStep);
                newPosition = new Area.Position(0, 0.5f);
            }
            area.SetParent(menuArea);
            elementsArea.Add(area);

            elementsArea[0].GoTo(newPosition,
                () =>
                {
                    topMenuIndex++;
                    if(backgroundIndex != null)
                        backgroundIndex++;
                    menuIndex++;
                    ChangeSelection();

                    isMoving = false;

                    UpdateNumberOfElements();

                }, true);

            if (verticalSeparation)
                elementsArea[0].Reduce(false, true, true);
            else
                elementsArea[0].Reduce(true, false, true);

            for (int i = 1; i < elementsArea.Count; i++)
            {
                elementsArea[i].GoTo(elementsArea[i - 1].centerPosition, true);
            }

            if (verticalSeparation)
                elementsArea[elementsArea.Count - 1].Grow(false, true, true);
            else
                elementsArea[elementsArea.Count - 1].Grow(true, false, true);
        } else
        {
            topMenuIndex--;
            if (backgroundIndex != null)
                backgroundIndex--;
            ChangeSelection();
            Area area;
            Area.Position newPosition;
            if (verticalSeparation)
            {
                area = new Area(0.5f, 0, 1, 1 / ((float)elementsToShow), menuArea.growStep, menuArea.moveStep);
                newPosition = new Area.Position(0.5f, 1);
            }
            else
            {
                area = new Area(0f, 0.5f, 1 / ((float)elementsToShow), 1, menuArea.growStep, menuArea.moveStep);
                newPosition = new Area.Position(1, 0.5f);
            }
            area.SetParent(menuArea);
            elementsArea.Insert(0, area);

            if (verticalSeparation)
                elementsArea[0].Grow(false, true, true);
            else
                elementsArea[0].Grow(true, false, true);


            for (int i = 0; i < elementsArea.Count - 1; i++)
            {
                elementsArea[i].GoTo(elementsArea[i + 1].centerPosition, true);
            }

            if (verticalSeparation)
                elementsArea[elementsArea.Count - 1].Reduce(false, true, true);
            else
                elementsArea[elementsArea.Count - 1].Reduce(true, false, true);
            elementsArea[elementsArea.Count - 1].GoTo(newPosition,
                () =>
                {
                    menuIndex--;
                    ChangeSelection();
                    isMoving = false;
                    UpdateNumberOfElements();
                }, true);
        }

    }

    /// <summary>
    /// Reset the defaults area of the menu.
    /// </summary>
    public void UpdateNumberOfElements()
    {
        elementsArea.Clear();

        for (int i=0; i < elementsToShow; i++)
        {
            //Debug.Log("Grow: " + menuArea.growStep);
            Area area;
            if (verticalSeparation)
                 area = new Area(0.5f,(i+0.5f)/((float)elementsToShow),1,1/((float)elementsToShow), menuArea.growStep, menuArea.moveStep);
            else
                area = new Area((i + 0.5f) / ((float)elementsToShow), 0.5f,  1 / ((float)elementsToShow),1, menuArea.growStep, menuArea.moveStep);
            area.SetParent(menuArea);
            elementsArea.Add(area);
        }

    }

    /// <summary>
    /// Function to calcule the position of each visible element and call DrawElement
    /// Should be call in OnGui when the menu has to be draw
    /// </summary>
    /// <see cref="DrawElement(Rect, int, bool)"/>
    /// <see cref="DrawFragmentElement(float, bool, Rect, int, bool)"/>
    /// <see cref="DrawBackgroundElement(Rect, int)"/>
    /// <see cref="DrawFragmentBackgroundElement(float, bool, Rect, int)"/>
    public override void Draw()
    {
        if (menuIndex == null)
            return;

        if (menuIndex.Max() == 0)
            return;

        DrawContext();

        if (elementsToShow >= menuIndex.Max())
        {
            for (int i=0; i<elementsToShow; i++)
            {
                if (backgroundIndex != null)
                {
                    DrawBackgroundElement(elementsArea[i].GetRect(), backgroundIndex.IndexMovedBy(i));
                }

                if (i < menuIndex.Max())
                {
                    DrawElement(elementsArea[i].GetRect(), i, i == menuIndex.Get());
                }
            }
        }
        else
        {
            for (int i = 0; i < elementsArea.Count; i++)
            {
                if (backgroundIndex != null)
                {
                    if (elementsArea[i].GetPercentSizeHeigth() != 1)
                        DrawFragmentBackgroundElement(elementsArea[i].GetPercentSizeHeigth(), i==0 ,elementsArea[i].GetRect(), backgroundIndex.IndexMovedBy(i));
                    else
                        DrawBackgroundElement(elementsArea[i].GetRect(), backgroundIndex.IndexMovedBy(i));
                }

                if (i < menuIndex.Max())
                {
                    if (elementsArea[i].GetPercentSizeHeigth() != 1)
                        DrawFragmentElement(elementsArea[i].GetPercentSizeHeigth(), i == 0, elementsArea[i].GetRect(), topMenuIndex.IndexMovedBy(i), topMenuIndex.IndexMovedBy(i) == menuIndex.Get());
                    else
                        DrawElement(elementsArea[i].GetRect(), topMenuIndex.IndexMovedBy(i), topMenuIndex.IndexMovedBy(i) == menuIndex.Get());
                }
            }

        }

    }

}
