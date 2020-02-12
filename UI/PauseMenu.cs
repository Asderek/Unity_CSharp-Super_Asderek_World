using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    private GameObject player;
    private UIManager manager;
    private GameManager gameManager;

    public Item[] items;

    public Texture background;
    public Texture backgroundBorder;
    private float borderheight = 70;

    public TextMenu options;

    public enum PauseOptions
    {
        Continue,
        Items,
        Save_exit
    }


    // Use this for initialization
    void Start () {
        manager = UIManager.GetInstance();
        gameManager = GameManager.GetInstance();
        player = GameObject.FindGameObjectWithTag("Player");

        options.Init(true, true);
    }
	
	// Update is called once per frame
	void Update ()
    { 
        if (ButtonManager.GetDown(ButtonManager.ButtonID.START,this))
        {
            manager.Pause();
        }

    }

    public void UpdateOnPause()
    {
        options.Update();

        if (ButtonManager.GetDown(ButtonManager.ButtonID.X,gameObject))
        { 
            switch ((PauseOptions)options.menuIndex.Get())
            {
                case PauseOptions.Continue:
                    manager.Pause();
                    break;

                case PauseOptions.Items:
                    break;

                case PauseOptions.Save_exit:
                    gameManager.ExitGame();
                    manager.ChangeMode(UIManager.Mode.START_GAME);
                    break;
            }
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.CIRCLE, gameObject))
        {
            manager.Pause();
        }

    }

    public void Display()
    {
        DrawBackGround();
        DrawItems();
        options.Draw();
    }

    protected void DrawBackGround()
    {
        /*BackGround = */
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill, true, 0);

        

        /*Background Border*/
        GUI.DrawTexture(new Rect(0, 0, Screen.width, borderheight), backgroundBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(0, 0, borderheight, Screen.height), backgroundBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(Screen.width-borderheight, 0, borderheight, Screen.height), backgroundBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(0, Screen.height - borderheight, Screen.width, borderheight), backgroundBorder, ScaleMode.StretchToFill, true, 0);
    }

    protected void DrawItems()
    {
       

    }

    public bool AddItem(Item item)
    {
        for(int i =0;i<items.Length;i++)
        {
            if (items[i].texture == null)
            {
                items[i] = item;
                return true;
            }
            
        }
        return false;


    }

    /*************************************************************************************************************************/
    //[Header("----------------------------------------------------------------------")]
    //Area
    //[Header("----------------------------------------------------------------------")]
    /*************************************************************************************************************************/


}
