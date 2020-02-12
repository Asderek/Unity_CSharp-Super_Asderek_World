using UnityEngine;
using System;
using Assets.Scripts;

[System.Serializable]
public class ActiveAbilitiesMenu: ClockMenu
{
    public enum Options
    {
        UP,
        RIGHT,
        DOWN,
        LEFT          
    }
    [Header("-----------------------------ActiveAbilitiesMenu-----------------------------------------")]
    public Area elementBorder;
    public Area elementContent;
    public Area buttonArea;

    public bool adjustToSquare = false;

    public Texture2D nullAbilityTexture;
    public Texture2D abilityBorder;
    public Texture2D cdOverlayTexture;
    private int numActiveAbilities = 4;
    public int numOfSlotsAvailable = 1;

    [Tooltip("Controls how the selected texture will be blink, textures should be enable to read/write")]
    public BlinkTexture.BlinkControl blinkControl;
    protected BlinkTexture selectedImage;

    private GameManager gameManager;
    private UIManager uiManager;
    private Ability.AbilitySettings[] abilities;
    public bool isActive;

    private Ability abilitiyMenu;

    /// <summary>
    /// Function to initialize some internal properties. Should be called in Start()
    /// </summary>
    public void Init()
    { 
        base.Init(new CircularIndex(numActiveAbilities));

        elementBorder.SetParent(menuArea);
        elementContent.SetParent(menuArea);

        gameManager = GameManager.GetInstance();
        uiManager = UIManager.GetInstance();
        abilitiyMenu = uiManager.ability;


        abilities = new Ability.AbilitySettings[numActiveAbilities];
        for (int i = 0; i < numActiveAbilities; i++)
        {
            abilities[i] = new Ability.AbilitySettings();
            abilities[i].texture = nullAbilityTexture;
        }

        foreach (var pair in abilitiyMenu.abilities)
        {
            if (pair.Value.isActivatable)
            {
                if ((pair.Value.saveInfo.obtained) && (pair.Value.saveInfo.isSelected))
                {
                    menuIndex.Set((int)pair.Value.saveInfo.selectLocation);
                    ChangeSelectedAbility(pair.Value);
                }
            }
        }
        menuIndex.Set(0);

        selectedImage = new BlinkTexture(abilities[0].texture, blinkControl);
        if (adjustToSquare)
            menuArea.AdjustToSquare();
    }

    /// <summary>
    /// Function to make to control of movement and resizing operations also to change the selected element.
    /// Should be called in Update() of Monobehavior always that menu will be displayed
    /// </summary>
    public override void Update()
    {

        int before = menuIndex.Get();
        base.Update();

        if (before != menuIndex.Get())
        {
                selectedImage = new BlinkTexture(abilities[menuIndex.Get()].texture, blinkControl);
        }

        selectedImage.Update();

        if (adjustToSquare)
            menuArea.AdjustToSquare();
    }

    public void UpdateBlink()
    {
        selectedImage.Update();
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

        switch (numOfSlotsAvailable)
        {
            case 1:
                if (elementIndex > 0)
                    return;
                break;
            case 2:
                if (elementIndex %2 == 0)
                    return;
                elementIndex--;
                break;
            case 3:
                if (elementIndex == 2)
                    return;
                break;
        }

        if (abilityBorder)
        {
            GUI.DrawTexture(elementBorder.GetRect(rect), abilityBorder, ScaleMode.ScaleToFit);
        }
        Area ultAnimalFull = new Area(elementContent.GetRect(rect));

        float percent = CoolDownManager.RemainingTimePercent(abilities[elementIndex].description.id.ToString());
        if (isActive && this.isActive)
        {
            GUI.DrawTexture(ultAnimalFull.GetRect(), selectedImage.GetTexture(), ScaleMode.ScaleToFit);
        }
        else
        {
            GUI.DrawTexture(ultAnimalFull.GetRect(), abilities[elementIndex].texture, ScaleMode.ScaleToFit);
        }
        if (percent > 0)
        {
            percent = percent / 2;
            GUIMethods.DrawOverlayTexture(elementBorder.GetRect(rect), null, cdOverlayTexture, percent, Options.UP);

            GUIMethods.DrawOverlayTexture(elementBorder.GetRect(rect), null, cdOverlayTexture, percent, Options.DOWN);

            GUIMethods.DrawOverlayTexture(elementBorder.GetRect(rect), null, cdOverlayTexture, percent, Options.LEFT);

            GUIMethods.DrawOverlayTexture(elementBorder.GetRect(rect), null, cdOverlayTexture, percent, Options.RIGHT);
            
        }

        if ((abilities[elementIndex].description.id != Ability.AbilityId.NONE) && ButtonManager.Get(ButtonManager.ButtonID.L2))
        {
            GUI.DrawTexture(buttonArea.GetRect(rect), ButtonManager.GetButtonTexture((ButtonManager.ButtonID)elementIndex));
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
        Options trySelect;
        ButtonManager buttonManager = ButtonManager.GetInstance();

        
        if (ButtonManager.GetDown(ButtonManager.ButtonID.DIRECT_RIGHT, this))
            trySelect = Options.RIGHT;
        else if (ButtonManager.GetDown(ButtonManager.ButtonID.DIRECT_LEFT, this))
            trySelect = Options.LEFT;
        else if (ButtonManager.GetDown(ButtonManager.ButtonID.DIRECT_UP, this))
            trySelect = Options.UP;
        else if (ButtonManager.GetDown(ButtonManager.ButtonID.DIRECT_DOWN, this))
            trySelect = Options.DOWN;
        else
            return 0;

        //Debug.Log("TRYSELECT = " + trySelect);

        switch (trySelect)
        {
            case Options.UP:
                break;
            case Options.DOWN:
                if (numOfSlotsAvailable % 2 != 0)
                    return 0;
                break;
            case Options.RIGHT:
            case Options.LEFT:
                if (numOfSlotsAvailable < 2)
                    return 0;
                break;
        }

        menuIndex.Set((int)trySelect);

        return 0;
    }

    public bool ChangeSelectedAbility(Ability.AbilitySettings ability)
    {
        //Debug.Log("Eq length: " + abilities.Length);
        int i;
        for (i =0; i < numActiveAbilities; i++)
        {
            //Debug.Log(abilities[i].json.id + " --> " + ability.json.id);
            if (abilities[i].description.id == ability.description.id)
            {
                break;
            }
        }

        int addIndex = menuIndex.Get();
        if (numOfSlotsAvailable == 2)
        {
            addIndex = menuIndex.IndexMovedBy(-1);
        }

        if (i < numActiveAbilities)
        {
            //Debug.Log("Found at: " + i);
            if (i == addIndex)
            {
                abilitiyMenu.abilities[abilities[addIndex].description.id].saveInfo.isSelected = false;
                DeselectAbility(i);
                return false;
            }
            else
                abilities[i] = abilities[addIndex];
        }
        else
        {
                abilitiyMenu.abilities[abilities[addIndex].description.id].saveInfo.isSelected = false;
        }

        abilities[addIndex] = ability;
        abilitiyMenu.abilities[ability.description.id].saveInfo.isSelected = true;
        abilitiyMenu.abilities[ability.description.id].saveInfo.selectLocation = (Options)addIndex;
        selectedImage = new BlinkTexture(ability.texture, blinkControl);
        return true;
    }

    public void DeselectAbility()
    {
        abilitiyMenu.abilities[abilities[menuIndex.Get()].description.id].saveInfo.isSelected = false;
        abilities[menuIndex.Get()] = new Ability.AbilitySettings();
        abilities[menuIndex.Get()].texture = nullAbilityTexture;
        selectedImage = new BlinkTexture(nullAbilityTexture, blinkControl);
    }

    public Ability.AbilityId GetHoveredAbility()
    {
        return abilities[menuIndex.Get()].description.id;
    }

    public void DeselectAbility(int index)
    {
        abilities[index] = new Ability.AbilitySettings();
        abilities[index].texture = nullAbilityTexture;
        if (index == menuIndex.Get())
            selectedImage = new BlinkTexture(nullAbilityTexture, blinkControl);

    }

    public bool TestAbility(Ability.AbilitySettings ability)
    {
        int addIndex = menuIndex.Get();
        if (numOfSlotsAvailable == 2)
        {
            addIndex = menuIndex.IndexMovedBy(-1);
        }

        int i;
        for (i = 0; i < numActiveAbilities; i++)
        {
            //Debug.Log(abilities[i].json.id + " --> " + ability.json.id);
            if (abilities[i].description.id == ability.description.id)
            {
                if (i == addIndex)
                {
                    return true;
                }
            }
        }
        
        return false;
    }


    public Area.Position GetSlot(Options opt)
    {
        if (numOfSlotsAvailable == 2)
        {
            switch (opt)
            {
                case Options.UP:
                    return GetArea((int)Options.RIGHT).centerPosition;
                case Options.RIGHT:
                    return GetArea((int)Options.LEFT).centerPosition;
            }
        }

        return GetArea((int)opt).centerPosition;
    }

    public bool HasSlot(Options opt)
    {
        switch (opt)
        {
            case Options.UP:
                return numOfSlotsAvailable != 2;

            case Options.DOWN:
                return numOfSlotsAvailable == 4;

            case Options.RIGHT:
            case Options.LEFT:
                return numOfSlotsAvailable > 1;
        }
        return false;
    }

    public void ChangeSelected(Options opt)
    {
        if (numOfSlotsAvailable == 2)
        {
            switch (opt)
            {
                case Options.UP:
                    menuIndex.Set((int)Options.LEFT);
                    break;
            }
        }

        menuIndex.Set((int)opt);
    }

    public Ability.AbilityId GetID(ActiveAbilitiesMenu.Options options)
    {
        return abilities[(int)options].description.id;
    }
}