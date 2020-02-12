using UnityEngine;

[System.Serializable]
public class LoadingMenu : ScrollMenu
{
    private GameManager gameManager;
    private UIManager uiManager;

    public Area time;
    public Area gameComplete;
    public TextureBox shrine;
    public TextureBox selected;

    public void Init()
    {
        gameManager = GameManager.GetInstance();
        uiManager = UIManager.GetInstance();
        base.Init(new CircularIndex(gameManager.saveJson.saves.Length),null,true);

        time.SetParent(menuArea);
        gameComplete.SetParent(menuArea);
        shrine.Init();
        selected.Init();

    }

    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
        if (elementIndex >= gameManager.saveJson.saves.Length)
            return;

        GUIStyle style = gameManager.TextStyle();
        style.fontSize = (int)( style.fontSize * menuArea.GetPercentSize());

        GUI.Label(time.GetRect(rect), "Play Time: " + gameManager.saveJson.saves[elementIndex].time.ToString(), style);
        GUI.Label(gameComplete.GetRect(rect), "Game Progress: " + gameManager.saveJson.saves[elementIndex].percentComplete + "%", style);

        shrine.contentTexture = uiManager.warp.GetTexture(gameManager.saveJson.saves[elementIndex].respawPoint);
        shrine.Draw(rect);

        if (isActive)
        {
            selected.Draw(rect);
        }

    }

    public override void Update()
    {
        int before = menuIndex.Get();
        base.Update();
        //if (before != menuIndex.Get())
        //{
        //    blink = new BlinkTexture(uiManager.warp.GetTexture(gameManager.saveJson.saves[menuIndex.Get()].respawPoint), blinkControl);
        //}
    }

    public override float GetAxis()
    {
        return ButtonManager.GetValue(ButtonManager.ButtonID.DIRECT_UP, ButtonManager.ButtonID.DIRECT_DOWN);
    }

    public override void DrawBackgroundElement(Rect rect, int index)
    {
    }

    public override void DrawFragmentBackgroundElement(float percentHeight, bool goingUp, Rect rect, int elementIndex)
    {
    }

    public override void DrawFragmentElement(float percentHeight, bool drawUp, Rect rect, int elementIndex, bool isActive)
    {
    }

    public override float GetPageScroll()
    {
        return 0;
    }
}