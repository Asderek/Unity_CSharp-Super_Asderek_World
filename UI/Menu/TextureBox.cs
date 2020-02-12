using UnityEngine;

[System.Serializable]
public class TextureBox : AbstractMenu
{
    public Area contentArea;
    public Texture2D contentTexture;

    public void Init()
    {
        base.Init(new LimitedIndex(0));
        contentArea.SetParent(menuArea);
    }

    public override void Draw()
    {
        base.DrawContext();
        if (contentTexture)
            GUI.DrawTexture(contentArea.GetRect(), contentTexture);
    }

    public void Draw(Rect rect)
    {
        if (contentTexture)
        {
            Area area = new Area(contentArea.GetRect(menuArea.GetRect(rect)));
            area.AdjustToSquare();
            GUI.DrawTexture(area.GetRect(), contentTexture);
        }

        if (border)
        {
            Area area = new Area(menuArea.GetRect(rect));
            area.AdjustToSquare();
            GUI.DrawTexture(area.GetRect(), border);
        }

        cardinalTextures.Draw(new Area(menuArea.GetRect(rect)));
    }

    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
    }

    public override float GetAxis()
    {
        return 0;
    }
}