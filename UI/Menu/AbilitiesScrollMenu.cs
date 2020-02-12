using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilitiesScrollMenu : ScrollMenu
{
    private List<Ability.AbilitySettings> elements;

    [Header("----------------------------------------------------------------------")]
    public Area elementImage;
    public Texture2D[] backgrounds;

    protected GUIStyle styleTitle;
    public Font font;
    public int fontSize;

    [Tooltip("Controls how the selected texture will be blink, textures should be enable to read/write")]
    public BlinkTexture.BlinkControl blinkControl;
    protected BlinkTexture selectedImage;

    public void Init(List<Ability.AbilitySettings> elements)
    {
        base.Init(new CircularIndex(elements.Count), new CircularIndex(backgrounds.Length));
        this.elements = elements;

        styleTitle = new GUIStyle();
        styleTitle.alignment = TextAnchor.MiddleCenter;
        styleTitle.fontSize = fontSize;
        styleTitle.font = font;
        styleTitle.wordWrap = true;
        styleTitle.normal.textColor = Color.white;

        if (elements.Count > 0)
        {
            selectedImage = new BlinkTexture(elements[0].texture, blinkControl);
        }

    }

    public override void Update()
    {
        int before = menuIndex.Get();
        base.Update();

        if (before != menuIndex.Get())
        {
            if (elements.Count > 0)
            {
                selectedImage = new BlinkTexture(elements[menuIndex.Get()].texture, blinkControl);
            }
        }

        selectedImage.Update();
        elementImage.AdjustToSquare();
    }

    public override void DrawBackgroundElement(Rect rect, int index)
    {
        if (index < 0)
            return;

        GUI.DrawTexture(rect, backgrounds[index]);
    }

    public override void DrawElement(Rect rect, int element, bool isActive)
    {
        if ((element >= elements.Count) || (element < 0))
            return;

        if (isActive)
        {
            GUI.DrawTexture(elementImage.GetRect(rect), selectedImage.GetTexture(), ScaleMode.ScaleToFit);
            //styleTitle.normal.textColor = Color.blue;
            //DrawTextResizing(elementText.GetRect(rect), elements[element].text, styleTitle, (int)(fontSize * menuArea.GetPercentSizeWidth()));
            //styleTitle.normal.textColor = Color.white;
        }
        else
        {
            GUI.DrawTexture(elementImage.GetRect(rect), elements[element].texture, ScaleMode.ScaleToFit);
            //DrawTextResizing(elementText.GetRect(rect), elements[element].text, styleTitle, (int)(fontSize * menuArea.GetPercentSizeWidth()));
        }
    }

    public override void DrawFragmentBackgroundElement(float percentHeight, bool drawUp, Rect rect, int element)
    {
        if ((element >= backgrounds.Length) || (element < 0))
            return;

        Rect rectText = new Rect(0, (drawUp) ? 0 : 1 - percentHeight, 1, percentHeight);
        GUI.DrawTextureWithTexCoords(rect, backgrounds[element], rectText, true);
    }

    public override void DrawFragmentElement(float percentHeight, bool drawUp, Rect rect, int element, bool isActive)
    {
        if (element >= elements.Count)
            return;

        if (isActive)
        {
            Rect rectText = new Rect(0, (drawUp) ? 0 : 1 - percentHeight, 1, percentHeight);
            GUI.DrawTextureWithTexCoords(elementImage.GetRect(rect), selectedImage.GetTexture(), rectText, true);

            //styleTitle.normal.textColor = Color.blue;

            //if (percentHeight > 0)
            //{
            //    if (drawUp)
            //        elementText.centerPosition.y = elementText.centerPosition.y * (percentHeight);
            //    else
            //        elementText.centerPosition.y = 1 - elementText.centerPosition.y * (percentHeight);

            //    DrawTextResizing(elementText.GetRect(rect), elements[element].text, styleTitle, (int)(fontSize * menuArea.GetPercentSizeWidth()));

            //    if (drawUp)
            //        elementText.centerPosition.y = elementText.centerPosition.y / (percentHeight);
            //    else
            //        elementText.centerPosition.y = (1 - elementText.centerPosition.y) / (percentHeight);
            //    styleTitle.normal.textColor = Color.white;
            //}

        }
        else
        {
            Rect rectText = new Rect(0, (drawUp) ? 0 : 1 - percentHeight, 1, percentHeight);
            GUI.DrawTextureWithTexCoords(elementImage.GetRect(rect), elements[element].texture, rectText, true);
            //if (percentHeight > 0)
            //{
            //    if (drawUp)
            //        elementText.centerPosition.y = elementText.centerPosition.y * (percentHeight);
            //    else
            //        elementText.centerPosition.y = 1 - elementText.centerPosition.y * (percentHeight);

            //    DrawTextResizing(elementText.GetRect(rect), elements[element].text, styleTitle, (int)(fontSize * menuArea.GetPercentSizeWidth()));

            //    if (drawUp)
            //        elementText.centerPosition.y = elementText.centerPosition.y / (percentHeight);
            //    else
            //        elementText.centerPosition.y = (1 - elementText.centerPosition.y) / (percentHeight);
            //}
        }
    }

    public override float GetAxis()
    {
        return -ButtonManager.GetValue(ButtonManager.ButtonID.DIRECT_DOWN, ButtonManager.ButtonID.DIRECT_UP);
    }

     public override float GetPageScroll()
    {
        return 0;
    }
}
