using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to exemplify a simple clock menu with only texture and border
/// </summary>
[System.Serializable]
public class SmallWeaponMenu : ClockMenu
{

    public Area onPlay, onPause;
    public Area pauseBG;

    [Tooltip ("Relative position and size of the border")]
    public Area elementBorder;
    [Tooltip ("Relative position and size of the content")]
    public Area elementContent;

    public Area weaponPauseBGArea;
    public Area weaponPauseArea;

    [Tooltip("Border's texture")]
    public Texture2D elementBorderTexture;

    [Tooltip("Controls how the selected texture will be blink, textures should be enable to read/write")]
    public BlinkTexture.BlinkControl blinkControl;
    protected BlinkTexture selectedImage;

    private GameManager gameManager;
    private UIManager uiManager;
    private Weapons weapons;

    private bool state = true;
    
    /// <summary>
    /// Function to initialize some internal properties. Should be called in Start()
    /// </summary>
    public void Init(int weaponsSize, Commandments.Weapon selectedWeapon) 
    {
        base.Init(new CircularIndex(weaponsSize));

        gameManager = GameManager.GetInstance();
        uiManager = UIManager.GetInstance();
        weapons = uiManager.weapon;

        elementContent.SetParent(onPlay);

        if (weaponsSize > 0)
        {
            selectedImage = new BlinkTexture(weapons.GetWeapon(selectedWeapon).elementActivated, blinkControl);
            menuIndex.Set(selectedWeapon.toInt());
        }

        onPlay.AdjustToSquare();
        onPause.AdjustToSquare();

        weaponPauseBGArea.SetParent(onPause);
        weaponPauseArea.SetParent(weaponPauseBGArea);
        pauseBG.SetParent(onPause);

    }

    /// <summary>
    /// Function to make to control of movement and resizing operations also to change the selected element.
    /// Should be called in Update() of Monobehavior always that menu will be displayed
    /// </summary>
    public void Update(int selectedWeapon)
    {
        if (uiManager.mode != UIManager.Mode.PAUSED)
            menuArea.centerPosition = gameManager.getCurrentCamera().GetTargetPosition();

        int before = menuIndex.Get();
        base.Update();

        if (before != menuIndex.Get())
        {
            selectedImage = new BlinkTexture( weapons.GetWeapon((Commandments.Weapon)menuIndex.Get()).elementActivated, blinkControl);
        }

        selectedImage.Update();
    }



    public override void DrawContext()
    {
        if (uiManager.mode == UIManager.Mode.PAUSED)
        {
            if (border)
            {
                GUI.DrawTexture(pauseBG.GetRect(), border);
            }
            int index = menuIndex.Get();
            if (weapons.GetWeapon((Commandments.Weapon)index).obtained)
            {
                if (weapons.GetWeapon((Commandments.Weapon)index).weaponTexture != null)
                {
                    GUI.DrawTexture(weaponPauseArea.GetRect(), weapons.GetWeapon((Commandments.Weapon)index).weaponTexture, ScaleMode.ScaleToFit);
                }
            }
        }
    }

    /// <summary>
    /// Function that will be called to draw each visible element on screen
    /// The occupied area calculation will be made in another place to separate the logic to show one menu element of the rest
    /// </summary>
    /// <param name="rect">Area to draw your menu element</param>
    /// <param name="elementIndex">Index of the element that should be drawn</param>
    /// <param name="isActive">True if that element is active. False otherwise</param>
    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
        if (elementBorderTexture)
        {
            GUI.DrawTexture(elementBorder.GetRect(rect),elementBorderTexture, ScaleMode.ScaleToFit);
        }

        if (isActive)
        {
            GUI.DrawTexture(elementContent.GetRect(rect), selectedImage.GetTexture(), ScaleMode.ScaleToFit);
        } else
        {
            if (weapons.GetWeapon((Commandments.Weapon)elementIndex).obtained)
                GUI.DrawTexture(elementContent.GetRect(rect), weapons.GetWeapon((Commandments.Weapon)elementIndex).elementActivated, ScaleMode.ScaleToFit);
            else
            {
                GUI.DrawTexture(elementContent.GetRect(rect), elementBorderTexture, ScaleMode.ScaleToFit);
                GUI.DrawTexture(elementContent.GetRect(rect), weapons.GetWeapon((Commandments.Weapon)elementIndex).elementDeactivated, ScaleMode.ScaleToFit);
            }
        }

    }

    /// <summary>
    /// Read Input Axis Horizontal to scroll
    /// </summary>
    /// <returns>Return:
    ///     <para>higher than zero if right arrow is pressed</para>
    ///     <para>lower than zero if left arrow is pressed</para>
    ///     <para>equal to zero if neither rigth or left arrow is pressed</para>
    /// </returns>
    public override float GetAxis()
    {
        //        return Input.GetAxis("Horizontal");

        if (menuArea.GetPercentSizeHeigth() != 1)
            return 0;

        float r_vertical = -ButtonManager.GetValue(ButtonManager.ButtonID.R_DOWN, ButtonManager.ButtonID.R_UP);
        float r_horizontal = ButtonManager.GetValue(ButtonManager.ButtonID.R_LEFT, ButtonManager.ButtonID.R_RIGHT);

        if (r_vertical == 0 && r_horizontal == 0)
        {
            return 0;
        }
        
        float decisionAngle = Mathf.Atan2(r_vertical, r_horizontal) * Mathf.Rad2Deg;

        int angle = (int)(decisionAngle + startAngle);
        angle = angle % 360;
        if (angle < 0)
            angle += 360;

        int newSelection = angle / 30;

        if (newSelection > 11)
        {
            newSelection = 0;
        }
        if (weapons.GetWeapon((Commandments.Weapon)newSelection).obtained)
            menuIndex.Set(newSelection);

        return 0;
    }

    public void ChangeState(UIManager.Mode mode)
    {
        //Debug.Log("Change to " + mode);
        switch (mode)
        {
            case UIManager.Mode.PAUSED:
                menuArea.CopyValues(onPause);
                break;

            default:
                menuArea.CopyValues(onPlay);
                break;
        }
    }

    public void ChangeWeaponIndex(Commandments.Weapon weapon)
    {
        menuIndex.Set(weapon.toInt());
        selectedImage = new BlinkTexture(weapons.GetWeapon(weapon).elementActivated, blinkControl);
    }

}
