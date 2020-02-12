using UnityEngine;

[System.Serializable]
public class TextMenu : AbstractMenu
{
    public string[] items;
    public Area itemArea;
    public Texture2D activeItemBgTexture;
    public bool titleStyle = false;

    private ButtonManager.ButtonID buttonDown;
    private ButtonManager.ButtonID buttonUp;

    public virtual void Init(bool vertical, bool circular)
    {
        base.Init(circular ? new CircularIndex(items.Length) : new LimitedIndex(items.Length) );
        verticalSeparation = vertical;

        if (verticalSeparation)
        {
            buttonUp = ButtonManager.ButtonID.DIRECT_DOWN;
            buttonDown = ButtonManager.ButtonID.DIRECT_UP;
        }
        else
        {
            buttonUp = ButtonManager.ButtonID.DIRECT_RIGHT;
            buttonDown = ButtonManager.ButtonID.DIRECT_LEFT;
        }
    }

    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
        if (elementIndex >= items.Length)
            return;

        GUIStyle style = titleStyle? GameManager.GetInstance().TitleStyle() : GameManager.GetInstance().TextStyle();
        style.fontSize = (int)(style.fontSize * Mathf.Min(menuArea.GetPercentSize(), 1f));
        rect = itemArea.GetRect(rect);
        if (isActive)
        {
            if (activeItemBgTexture)
            {
                Vector2 size = style.CalcSize(new GUIContent(items[elementIndex]));
                Rect rectText = new Rect(size.x / rect.width / 2, 0, size.x / rect.width, 1);
                GUI.DrawTextureWithTexCoords(new Rect(rect.x + rect.width / 2 - size.x / 2, rect.y, size.x, rect.height), activeItemBgTexture, rectText, true);
            }
        }
        GUI.Label(rect, items[elementIndex], style);
    }

    public override float GetAxis()
    {
        return ButtonManager.GetValue(buttonDown, buttonUp);
    }

    public void ChangeButtons(ButtonManager.ButtonID down, ButtonManager.ButtonID up)
    {
        buttonDown = down;
        buttonUp = up;
    }

}