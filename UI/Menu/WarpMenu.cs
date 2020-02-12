using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WarpMenu: AbstractMenu
{
    [System.Serializable]
    public class SceneMenu : ScrollMenu
    {
        public List<SceneInfo> list; 
        public Area elementArea;
        public Texture2D elementBorder;

        public override void DrawBackgroundElement(Rect rect, int index)
        {

        }

        public override void DrawElement(Rect rect, int elementIndex, bool isActive)
        {
            Area area = new Area(rect);
            elementArea.SetParent(area);
            //elementArea.AdjustToSquare();
            if (isActive)
            {
                if (elementBorder != null)
                    GUI.DrawTexture(rect, elementBorder);
            }
            GUI.DrawTexture(elementArea.GetRect(), list[elementIndex].warpImg);
            
        }

        public override void DrawFragmentBackgroundElement(float percentHeight, bool goingUp, Rect rect, int elementIndex)
        {
        }

        public override void DrawFragmentElement(float percentHeight, bool drawUp, Rect rect, int elementIndex, bool isActive)
        {
            if (elementIndex >= list.Count)
                return;

            /*if (isActive)
            {
                Rect rectText = new Rect(0, (drawUp) ? 0 : 1 - percentHeight, 1, percentHeight);
                GUI.DrawTextureWithTexCoords(elementArea.GetRect(rect), selectedImage.GetTexture(), rectText, true);
            }*/
            Area area = new Area(rect);
            elementArea.SetParent(area);
            Rect rectText = new Rect((!drawUp) ? 0 : 1 - percentHeight, 0, percentHeight, 1);
            GUI.DrawTextureWithTexCoords(elementArea.GetRect(), list[elementIndex].warpImg, rectText, true);
        }

        public override float GetAxis()
        {
            return ButtonManager.GetValue(ButtonManager.ButtonID.DIRECT_LEFT, ButtonManager.ButtonID.DIRECT_RIGHT);
        }

        public override float GetPageScroll()
        {
            return 0;// ButtonManager.GetValue(ButtonManager.ButtonID.L1, ButtonManager.ButtonID.R1);
        }
    }

    [System.Serializable]
    public class ShrineMenu : ScrollMenu
    {
        public List<ShrineInfo> list;
        public Area elementArea;
        public Texture2D elementBorder;
        public Texture2D bg;

        public override void DrawBackgroundElement(Rect rect, int index)
        {
        }

        public override void DrawElement(Rect rect, int elementIndex, bool isActive)
        {
            GUIStyle style = GameManager.GetInstance().TextStyle();

            if (isActive)
            {
                //if (elementBorder != null)
                //    GUI.DrawTexture(rect, elementBorder);
                if (bg)
                {
                    Vector2 size = style.CalcSize(new GUIContent(list[elementIndex].shrineName));
                    Rect rectText = new Rect(size.x / rect.width/2, 0, size.x/rect.width, 1);
                    GUI.DrawTextureWithTexCoords(new Rect(rect.x + rect.width / 2 - size.x / 2, rect.y, size.x, rect.height), bg, rectText, true);
                }
            }
            GUI.Label(elementArea.GetRect(rect), list[elementIndex].shrineName, style);
            
        }

        public override void DrawFragmentBackgroundElement(float percentHeight, bool goingUp, Rect rect, int elementIndex)
        {
        }

        public override void DrawFragmentElement(float percentHeight, bool drawUp, Rect rect, int elementIndex, bool isActive)
        {
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

    public SceneMenu topMenu;
    public ShrineMenu leftMenu;

    public Area shrineTextureArea;
    public Area shrineTextureBorderArea;
    public Texture2D shrineTextureBorder;


    public void InitScene()
    {
        topMenu.menuArea.SetParent(menuArea);
        topMenu.Init(new CircularIndex(topMenu.list.Count), null, false);
    }


    public void InitShrines()
    {
        leftMenu.menuArea.SetParent(menuArea);
        leftMenu.Init(new CircularIndex(leftMenu.list.Count), new LimitedIndex(1), true);
        shrineTextureBorderArea.SetParent(menuArea);
        shrineTextureArea.SetParent(shrineTextureBorderArea);
    }

    public override void Update()
    {
        topMenu.Update();
        leftMenu.Update();
    }
   
    public override void Draw()
    {
        base.Draw();
        topMenu.Draw();
        leftMenu.Draw();

        if (leftMenu.list[leftMenu.menuIndex.Get()].shrineTexture != null)
        {
            GUI.DrawTexture(shrineTextureArea.GetRect(), leftMenu.list[leftMenu.menuIndex.Get()].shrineTexture);
        }

        if (shrineTextureBorder)
        {
            GUI.DrawTexture(shrineTextureBorderArea.GetRect(), shrineTextureBorder);
        }
    }

    public override float GetAxis()
    {
        return 0;
    }

    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
    }
    

}