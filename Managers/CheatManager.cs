using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts;

[System.Serializable]
public class CheatManager
{
    public enum Cheats
    {
        AllWeapons,
        LeaveDeathWeapon,
        LeaveWaterWeapon
    }

    [System.Serializable]
    public struct ButtonList
    {
        public List<ButtonManager.ButtonID> buttonList;

        //[HideInInspector]
        public string converted;
        
    }

    [System.Serializable]
    public class CheatCodes : SerializableDictionary<Cheats, ButtonList> {}

    public CheatCodes codes;
    private string dictionary = "abcdefghijklmnopqrstuvwxyz";


    public void Init()
    {
        foreach (Cheats entry in Enum.GetValues(typeof(Cheats)))
        {
            ButtonList b = codes[entry];
            b.converted = ConvertButton(new List<ButtonManager.ButtonID>(codes[entry].buttonList));
            codes[entry] = b;
        }
    }


    public bool CheckInput(List<ButtonManager.ButtonID> history)
    {
        string strHistory = ConvertButton(history);

        foreach (Cheats entry in Enum.GetValues(typeof(Cheats)))
        {
            if (strHistory.Contains(codes[entry].converted))
            { 
                ApplyCode(entry);
                return true;
            }
        }
        return false;
    }

    public string ConvertButton(List<ButtonManager.ButtonID> buttons)
    {
        string code = "";
        foreach (ButtonManager.ButtonID id in buttons)
        {
            if ((int)id < dictionary.Length)
                code = code + dictionary[(int)id];
            else
                code = code + "*";
        }
        return code;
    }

    public void ApplyCode(Cheats cheat)
    {
        //return;
        //Debug.Log("ApplyCode: " + cheat);
        switch(cheat)
        {
            case Cheats.AllWeapons:
                for (Commandments.Element element = Commandments.Element.EARTH; element < Commandments.Element.NEUTRAL; element++)
                {
                    UIManager.GetInstance().weapon.ObtainWeapon(element);
                }
                break;
            case Cheats.LeaveDeathWeapon:
                for (Commandments.Element element = Commandments.Element.EARTH; element < Commandments.Element.NEUTRAL; element++)
                {
                    UIManager.GetInstance().weapon.ObtainWeapon(element,false);
                }
                UIManager.GetInstance().weapon.ObtainWeapon(Commandments.Element.DEATH);
                UIManager.GetInstance().weapon.SetSelectedWeapon(Commandments.Element.DEATH.toWeapon());
                break;
            case Cheats.LeaveWaterWeapon:
                for (Commandments.Element element = Commandments.Element.EARTH; element < Commandments.Element.NEUTRAL; element++)
                {
                    UIManager.GetInstance().weapon.ObtainWeapon(element, false);
                }
                UIManager.GetInstance().weapon.ObtainWeapon(Commandments.Element.WATER);
                UIManager.GetInstance().weapon.SetSelectedWeapon(Commandments.Element.WATER.toWeapon());
                break;
        }
    }

}