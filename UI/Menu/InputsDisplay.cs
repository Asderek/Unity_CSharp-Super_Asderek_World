using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InputsDisplay : AbstractMenu
{
    [System.Serializable]
    public struct InputDescription
    {
        public ButtonManager.ButtonID id;
        public string text;
    }

    public Area buttonArea;
    public Area textArea;
    public List<InputDescription> descriptions;

    public void Init()
    {
        base.Init(new LimitedIndex(descriptions.Count));
    }


    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
        GUI.DrawTexture(buttonArea.GetRect(rect), ButtonManager.GetButtonTexture(descriptions[elementIndex].id), ScaleMode.ScaleToFit);
        AbstractMenu.Label(textArea.GetRect(rect), descriptions[elementIndex].text, textArea.GetPercentSize()*0.7f);
    }

    public override float GetAxis()
    {
        return 0;
    }
}