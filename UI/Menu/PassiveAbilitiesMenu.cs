using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PassiveAbilitiesMenu : AbstractMenu
{
    [Header("----------------------------PassiveAbilitiesMenu------------------------------------------")]
    public int maxSlots = 10;
    public int numberOfSlots = 2;

    public Area elementBorder;
    public Area elementContent;

    public Texture2D nullAbilityTexture;
    public Texture2D abilityBorder;

    private List<Ability.AbilitySettings> abilities;

    public BlinkTexture.BlinkControl blinkControl;
    protected BlinkTexture selectedImage;


    private Ability abilitiyMenu;

    public bool isActive;

    public virtual void Init()
    {
        base.Init( new CircularIndex(maxSlots));
        abilitiyMenu = UIManager.GetInstance().ability;
        verticalSeparation = false;
        selectedImage = new BlinkTexture(nullAbilityTexture, blinkControl);

        abilities = new List<Ability.AbilitySettings>();


        foreach (var pair in abilitiyMenu.abilities)
        {
            if (!pair.Value.isActivatable)
            {
                if ((pair.Value.saveInfo.obtained) && (pair.Value.saveInfo.isSelected))
                {
                    SelectAbility(pair.Value);
                }
            }
        }
    }

    public override void Update()
    {
        int before = menuIndex.Get();
        base.Update();
        if (menuIndex.Get() >= abilities.Count)
        {
            if (before == 0)
            {
                menuIndex.Set(abilities.Count - 1);
            } else
            {
                menuIndex.Set(0);
            }
        }

        if (before != menuIndex.Get())
        {
            selectedImage = new BlinkTexture(abilities[menuIndex.Get()].texture, blinkControl);
        }
        selectedImage.Update();
    }

    public void UpdateBlink()
    {
        selectedImage.Update();
    }

    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
        elementIndex -= (maxSlots - numberOfSlots) / 2;

        if (elementIndex < 0)
            return;
        if (elementIndex >= numberOfSlots)
            return;

        if (numberOfSlots % 2 != 0)
            rect.x += rect.width / 2;

        if (abilityBorder)
        {
            GUI.DrawTexture(elementBorder.GetRect(rect), abilityBorder, ScaleMode.ScaleToFit);
        }

        if (elementIndex >= abilities.Count)
        {
            GUI.DrawTexture(elementContent.GetRect(rect), nullAbilityTexture, ScaleMode.ScaleToFit);
        } else if ((elementIndex == menuIndex.Get()) && this.isActive)
        { 
            GUI.DrawTexture(elementContent.GetRect(rect), selectedImage.GetTexture(), ScaleMode.ScaleToFit);
        }
        else
        { 
            GUI.DrawTexture(elementContent.GetRect(rect),abilities[elementIndex].texture, ScaleMode.ScaleToFit);
        }
    }

    public override float GetAxis()
    {
        return ButtonManager.GetValue(ButtonManager.ButtonID.DIRECT_LEFT, ButtonManager.ButtonID.DIRECT_RIGHT);
    }

    public bool IsSelectable(Ability.AbilitySettings ability)
    {
        if (ability.saveInfo.isSelected)
            return false;


        if (abilities.Count == numberOfSlots)
            return false;

        return true;
    }

    public bool SelectAbility(Ability.AbilitySettings ability)
    {
        if ((abilities.Count == numberOfSlots) && (!ability.saveInfo.isSelected))
            return false;

        int i;
        for (i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].description.id == ability.description.id)
            {
                break;
            }
        }

        if (i < abilities.Count)
        {
            abilities.Remove(abilities[i]);
            abilitiyMenu.abilities[ability.description.id].saveInfo.isSelected = false;
            if (abilities.Count != 0)
            {
                menuIndex.Set(0);
                selectedImage = new BlinkTexture(abilities[0].texture, blinkControl);
            }
            return true;
        }
        
        abilitiyMenu.abilities[ability.description.id].saveInfo.isSelected = true;
        

        if (abilities.Count >= numberOfSlots)
            return false;

        abilities.Add(ability);
        if (abilities.Count == 1)
        {
            selectedImage = new BlinkTexture(ability.texture, blinkControl);
        }
        return true;
    }

    public void DeselectAbility()
    {
        if (menuIndex.Get() >= numberOfSlots)
            return;

        abilities.Remove(abilities[menuIndex.Get()]);
        if (abilities.Count != 0)
        {
            menuIndex.Set(0);
            selectedImage = new BlinkTexture(abilities[0].texture, blinkControl);
        }
    }

    public void DeselectAbility(Ability.AbilitySettings ability)
    {
        abilities.Remove(ability);
    }

    public Area.Position GetNextSlot()
    {
        return GetArea(Mathf.Min(abilities.Count,numberOfSlots-1)).centerPosition;
    }

    public override Area GetArea(int elementIndex)
    {

        elementIndex += (maxSlots - numberOfSlots) / 2;
        Rect rect = base.GetArea(elementIndex).GetRect();
        
        if (numberOfSlots % 2 != 0)
            rect.x -= rect.width / 2;

        return new Area(rect);
    }

    public int GetNumberOfSelectedAbilities()
    {
        return abilities.Count;
    }

    public Ability.AbilityId GetHoveredAbility()
    {
        return abilities[menuIndex.Get()].description.id;
    }
}