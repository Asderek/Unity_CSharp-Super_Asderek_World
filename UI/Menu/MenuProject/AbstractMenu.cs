using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a basic abstraction of a minimal Menu, defining a minimal Menu to personalize into extensions
/// </summary>
[System.Serializable]
public abstract class AbstractMenu  {

    [Tooltip("Whole menu area")]
    public Area menuArea;
    [Tooltip("Whole menu area")]
    public Area innerArea;

    [Tooltip ("Texture draw into all menu area")]
    public Texture2D border;
    public CardinalTextures cardinalTextures;

    [HideInInspector]
    public LimitedIndex menuIndex;               // current selected menu element

    protected bool verticalSeparation;              // separation is vertical or horizontal
    protected int updateDelay;                      // number of updates after a index change before accept another
    protected int qtdUpdates;                       // numver of updates since the last index change

    /// <summary>
    /// Menus should read in Updates same Input to change the element selected
    /// </summary>
    /// <returns>Should return:
    ///     <para>higher than zero to increment - Increment Index - Element selected on Screen down</para>
    ///     <para>lower than zero to decrement - Decrement Index - Element selected on Screen up</para>
    ///     <para>equal to zero to keep the value</para>
    /// </returns>
    public abstract float GetAxis();

    /// <summary>
    /// Function that will be called to draw each visible element on screen
    /// The occupied area calculation will be made in another place to separate the logic to show one menu element of the rest
    /// </summary>
    /// <param name="rect">Area to draw your menu element</param>
    /// <param name="elementIndex">Index of the element that should be drawn</param>
    /// <param name="isActive">True if that element is active. False otherwise</param>
    public abstract void DrawElement(Rect rect, int elementIndex, bool isActive);

    /// <summary>
    /// Function to initialize some internal properties. Should be called in Start()
    /// </summary>
    /// <param name="Index">Index to control the menu index</param>
    public virtual void Init (LimitedIndex Index)
    {
        innerArea.SetParent(menuArea);
        verticalSeparation = true;
        updateDelay = 15;
        qtdUpdates = 0;
        menuIndex = Index;
    }

    public virtual void ChangeSelection()
    {

    }

    /// <summary>
    /// Function to make to control of movement and resizing operations also to change the selected element
    /// Should be called in Update() of Monobehavior always that menu will be displayed
    /// </summary>
    public virtual void Update ()
    {
        float axis = GetAxis();

        menuArea.Update();
        innerArea.Update();

        if (axis == 0)
        {
            qtdUpdates = updateDelay;
            return;
        }

        if (qtdUpdates < updateDelay)
        {
            qtdUpdates++;
            return;
        }

        if (axis > 0)
        {
            menuIndex++;
            ChangeSelection();
            qtdUpdates = 0;
        }else if (axis < 0)
        {
            menuIndex--;
            ChangeSelection();
            qtdUpdates = 0;
        }
    }


    /// <summary>
    /// Function to draw the menu border 
    /// </summary>
    public virtual void DrawContext()
    {
        if (border)
        {
            GUI.DrawTexture(menuArea.GetRect(), border);
        }

        cardinalTextures.Draw(menuArea);
    }

    /// <summary>
    /// Function to calcule the position of each visible element and call DrawElement
    /// Should be call in OnGui when the menu has to be draw
    /// </summary>
    /// <see cref="DrawElement(Rect, int, bool)"/>
    public virtual void Draw()
    {
        DrawContext();

        if (menuIndex == null)
            return;

        if (menuIndex.Max() == 0)
            return;

        Rect rect = innerArea.GetRect();

        if (verticalSeparation)
        {
            rect.height = rect.height / ((float)menuIndex.Max());

            for (int i=0; i<menuIndex.Max(); i++)
            {
                DrawElement(rect, i, menuIndex.Get() == i);
                rect.y += rect.height;
            }
        } else
        {
            rect.width = rect.width/ ((float)menuIndex.Max());

            for (int i = 0; i < menuIndex.Max(); i++)
            {
                DrawElement(rect, i, menuIndex.Get() == i);
                rect.x += rect.width;
            }
        }
    }

    /// <summary>
    /// The GUI.DrawTexture will be automatic adjust by the rect. However GUI.Label not will work properly, this function adapt the Draw of some text into one Area moving and resizing 
    /// </summary>
    public virtual void DrawTextResizing (Rect rect, string text, GUIStyle style, int fontSize)
    {
        style.fontSize = fontSize;
        float heigth = style.CalcHeight(new GUIContent(text), rect.width);
        if (heigth <= rect.height)
            GUI.Label(rect, text, style);
    }

    /// <summary>
    /// Gets the index of selected element.
    /// </summary>
    /// <returns></returns>
    public int GetIndex ()
    {
        return menuIndex.Get();
    }

    public virtual Area GetArea (int element)
    {
        if (element >= menuIndex.Max())
            return new Area(0, 0, 1, 1);

        Rect rect = innerArea.GetRect();

        if (verticalSeparation)
        {
            rect.height = rect.height / ((float)menuIndex.Max());
            rect.y += element* rect.height;            
        } else
        {
            rect.width = rect.width / ((float)menuIndex.Max());
            rect.x += rect.width*element;
        }

        return new Area(rect);
    }


    public static void Label(Rect rect, string text, float percentSize, bool useTitleStyle = false)
    {
        GUIStyle style = useTitleStyle ? GameManager.GetInstance().TitleStyle() : GameManager.GetInstance().TextStyle();
        style.fontSize = (int)(style.fontSize * percentSize);
        GUI.Label(rect, text, style);
    }

}
