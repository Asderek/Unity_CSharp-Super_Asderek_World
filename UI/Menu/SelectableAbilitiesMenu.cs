using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class SelectableAbilitiesMenu : AbstractMultilayerMenu
{
    private Ability.AbilityDictionary abilities;
    private List<Ability.AbilitySettings> activeList;

    [Header("-----------------------------SelectMenu-----------------------------------------")]
    public int rows;
    public int columns;
    public Area elementContent;
    public Area elementBorder;

    public Texture2D nullAbilityTexture;
    public Texture2D selectedBorder;
    public Area nullArea; 

    public BlinkTexture.BlinkControl blinkControl;
    protected BlinkTexture selectedBorderBlinked;

    public Func<Ability.AbilitySettings,bool> isSelectable = (Ability.AbilitySettings ability)=>
    {
        return (ability.isActivatable) && (ability.description.id != Ability.AbilityId.NONE);
    };

    public bool isActive = true;
    private Ability abilitiyMenu;
    private LimitedIndex lastRow;
    private LimitedIndex lastColumn;

    public virtual void Init(ref Ability.AbilityDictionary abilities)
    {
        this.abilities = abilities;
        abilitiyMenu = UIManager.GetInstance().ability;
        activeList = new List<Ability.AbilitySettings>();

        foreach (KeyValuePair<Ability.AbilityId, Ability.AbilitySettings> pair in abilities)
        {
            if (isSelectable(pair.Value))
            {
                activeList.Add(pair.Value);
            }
        }
        base.Init(new CircularIndex(rows), new CircularIndex(columns));
        selectedBorderBlinked = new BlinkTexture(selectedBorder, blinkControl);
        Update();

        lastRow = rowIndex.OffsetCopy(0);
        lastColumn = columnIndex.OffsetCopy(0);
        ChangeSelection();
    }

    public void CheckIndexes()
    {
        if ((rows != rowIndex.Max()) || (columns != columnIndex.Max()))
        {
            base.Init(new CircularIndex(rows), new CircularIndex(columns));
        }
    }

    public void Update()
    {
        base.Update();
        selectedBorderBlinked.Update();
    }

    public override void ChangeSelection()
    {
        base.ChangeSelection();
        int index = rowIndex.Get() * columnIndex.Max() + columnIndex.Get();
        if (index >= activeList.Count)
        {
            rowIndex.Set(lastRow.Get());
            columnIndex.Set(lastColumn.Get());
            return;
        }

        lastRow = rowIndex.OffsetCopy(0);
        lastColumn = columnIndex.OffsetCopy(0);

    }

    public override void DrawElement(Rect rect, int row, int column, bool isActive)
    {
        int index = row * columnIndex.Max() + column;
        if ( index >= activeList.Count)
            return;

        if (row % 2 == 1)
            rect.x += rect.width*0.2f;

        //Debug.Log("DrawElement["+row+","+column+"]: " + activeList[index].json.id + " is obtained -> " + activeList[index].obtained);
        if (isActive)
        {
            if (this.isActive)
            //    GUI.DrawTexture(elementBorder.GetRect(rect), selectedBorderBlinked.GetTexture(), ScaleMode.ScaleToFit);
            //else
                GUI.DrawTexture(elementBorder.GetRect(rect), selectedBorder, ScaleMode.ScaleToFit);
        }

        if ( activeList[index].saveInfo.obtained)
        {
            if ( abilitiyMenu.abilities[activeList[index].description.id].saveInfo.isSelected)
                GUI.DrawTexture(elementContent.GetRect(rect), activeList[index].textureSelected, ScaleMode.ScaleToFit);
            else
                GUI.DrawTexture(elementContent.GetRect(rect), activeList[index].texture, ScaleMode.ScaleToFit);
        } else
        {
            GUI.DrawTexture(nullArea.GetRect(elementContent.GetRect(rect)), nullAbilityTexture, ScaleMode.ScaleToFit);
        }



    }

    public Ability.AbilitySettings GetSelected()
    {
        return activeList[rowIndex.Get() * columnIndex.Max() + columnIndex.Get()];
    }

    public Area GetSelectedArea()
    {
        return GetArea(rowIndex.Get(),columnIndex.Get());
    }

    public override float GetAxisHorizontal()
    {
        return ButtonManager.GetValue(ButtonManager.ButtonID.DIRECT_LEFT, ButtonManager.ButtonID.DIRECT_RIGHT);
    }

    public override float GetAxisVertical()
    {
        return ButtonManager.GetValue(ButtonManager.ButtonID.DIRECT_DOWN, ButtonManager.ButtonID.DIRECT_UP);
    }

    public Ability.AbilityId GetHoveredAbility()
    {
        return GetSelected().description.id;
    }
}