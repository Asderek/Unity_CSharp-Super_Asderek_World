using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;

public class BarDisplay : MonoBehaviour
{

    public Area playTotalArea;


    [Header("------------------------------- ULT AREA ---------------------------------------")]

    public Area ultTabArea;
    public Area ultAnimalArea;

    public Texture2D ultBG;


    public Texture hpFull;
    public Texture hpEmpty;

    public Texture hpEmptyPlay;
    public Texture lifeBar;
    public Texture manaBar;
    public Texture manaBarCharging;


    [Header("------------------------------- BAR AREA ---------------------------------------")]

    public Area barsArea;
    public Area hpBarBGArea;
    public Area mpBarBGArea;
    public Area hpBarArea;
    public Area mpBarArea;

    public Texture2D bgBar;
    public Texture2D emptyBar;
    public Texture2D hpBar;
    public Texture2D mpBar;

    [Header("------------------------------- BUFF AREA ---------------------------------------")]
    public Area buffArea;
    public int maxBuffsCount = 5;


    [System.Serializable]
    public class ModifiersTextures : SerializableDictionary<Commandments.Modifiers, Texture2D> { }

    public ModifiersTextures modTextures;

    private Area buffLine;
    private Area deBuffLine;
    private List<Area> firstLine;
    private List<Area> secondLine;

    [Header("------------------------------- MISC AREA ---------------------------------------")]

    public BarDisplaySize size = BarDisplaySize.Mini;

    private Vector2 border = new Vector2(52, 36);
    private Vector2 lifePosition = new Vector2(132, 33);
    private Vector2 manaOrigin = new Vector2(139, 56.49f);


    //public AreaUtility buffs;
    private Asderek player;

    public enum BarDisplaySize
    {
        Full,
        Mini,
    };

    private bool change = true;
    private float hp = 1f;
    private float mp = 1f;
    public float charging = 0.2f;

    private UIManager manager;

    private int firstModifier;


    // Use this for initialization
    void Start()
    {
        player = (GameObject.FindGameObjectWithTag("Player")).GetComponent<Asderek>();

        
        ultTabArea.SetParent(playTotalArea);
        ultAnimalArea.SetParent(ultTabArea);
        barsArea.SetParent(playTotalArea);
        hpBarBGArea.SetParent(barsArea);
        mpBarBGArea.SetParent(barsArea);
        hpBarArea.SetParent(hpBarBGArea);
        mpBarArea.SetParent(mpBarBGArea);

        manager = UIManager.GetInstance();
        manager.weapon.ChangeUlt(1);
        playTotalArea.AdjustToSquare();

        /***********************************/
        buffArea.SetParent(playTotalArea);
        buffLine = new Area(0.5f, 0.25f, 1, 0.5f);
        deBuffLine = new Area(0.5f, 0.75f, 1, 0.5f);

        buffLine.SetParent(buffArea);
        deBuffLine.SetParent(buffArea);
        firstLine = new List<Area>();
        for (int i = 0; i < maxBuffsCount; i++)
        {
            firstLine.Add(new Area((float)i/(float)maxBuffsCount,0.5f,1.0f/(float)maxBuffsCount,1));
            firstLine[i].SetParent(buffLine);
        }
        secondLine = new List<Area>();
        for (int i = 0; i < maxBuffsCount; i++)
        {
            secondLine.Add(new Area((float)i / (float)maxBuffsCount, 0.5f, 1.0f / (float)maxBuffsCount, 1));
            secondLine[i].SetParent(deBuffLine);
        }
    /***********************************/
}

    // Update is called once per frame
    void Update()
    {

    }

    public void Display()
    {
        DrawBarsPlay();
        DrawUltPlay();
        DrawBuffPlay();

        //GUI.DrawTextureWithTexCoords()

        /* GUI.DrawTexture(new Rect(border.x, border.y, hpEmptyPlay.width, hpEmptyPlay.height), hpEmptyPlay);

         Rect rectSpace = new Rect(border.x + lifePosition.x, border.y + lifePosition.y, lifeBar.width * hp, lifeBar.height);
         Rect rectText = new Rect(0, 0, hp, 1);
         GUI.DrawTextureWithTexCoords(rectSpace, lifeBar, rectText, true);



          rectSpace = new Rect(border.x + manaOrigin.x, border.y + manaOrigin.y, manaBar.width * mp, manaBar.height);
          rectText = new Rect(0, 0, mp, 1);
         GUI.DrawTextureWithTexCoords(rectSpace, manaBar, rectText, true);

         */
    }

    public void DrawBarsPlay()
    {
        GUI.DrawTexture(hpBarBGArea.GetRect(),bgBar);
        GUI.DrawTexture(mpBarBGArea.GetRect(),bgBar);
        GUI.DrawTexture(hpBarArea.GetRect(), emptyBar);
        GUI.DrawTexture(mpBarArea.GetRect(), emptyBar);

        Area currentHp = new Area(hpBarArea);
        currentHp.SetParent(hpBarBGArea);
        currentHp.size.width *= hp;
        currentHp.centerPosition.x = hpBarArea.centerPosition.x - hpBarArea.size.width/2 + currentHp.size.width/2;
        Rect rectText = new Rect(0, 0, hp, 1);

        GUI.DrawTextureWithTexCoords(currentHp.GetRect(), hpBar, rectText, true);

        Area currentMp = new Area(mpBarArea);
        currentMp.SetParent(mpBarBGArea);
        currentMp.size.width *= mp;
        currentMp.centerPosition.x = mpBarArea.centerPosition.x - mpBarArea.size.width / 2 + currentMp.size.width / 2;
        rectText = new Rect(0, 0, mp, 1);


        GUI.DrawTextureWithTexCoords(currentMp.GetRect(), mpBar, rectText, true);

    }

    public void DrawUltPlay()
    {
        Weapons.WeaponSettings weaponSettings = manager.weapon.getCurrentWeapon();
        GUI.DrawTexture(ultTabArea.GetRect(), ultBG);

        GUI.DrawTexture(ultAnimalArea.GetRect(), weaponSettings.animalDeactivated);

        Area ultAnimalFull = new Area(ultAnimalArea);
        ultAnimalFull.SetParent(ultTabArea);
        ultAnimalFull.centerPosition.y -= (ultAnimalFull.centerPosition.y - (1 - manager.weapon.GetUlt() / 2)) * ultAnimalFull.size.height;
        ultAnimalFull.size.height *= manager.weapon.GetUlt();
        Rect rectText = new Rect(0, 0, 1, manager.weapon.GetUlt());

        GUI.DrawTextureWithTexCoords(ultAnimalFull.GetRect(), weaponSettings.animalActivated, rectText, true);

    }

    public void ChangeHP(float percent)
    {
        this.hp = percent;
    }

    public void ChangeMP(float percent)
    {
        this.mp = percent;
    }

    public void SetHPBarSize(BarDisplaySize size)
    {
        this.size = size;

    }

    public void DrawBuffPlay()
    {
        if (player.modifiers.Count == 0)
            return;

        //print((( player.buffs[0] + " - " + player.buffs[0].cooldown.isOnCD());
        int firstLineCount = 0;
        int secondLineCount = 0;

        for (int i = 0; i < player.modifiers.Count; i++)
        {
            if (CoolDownManager.RemainingTimeAbsolute("mod_" + player.modifiers[i].ToString()) == float.PositiveInfinity && player.modifiers[i].isBuff())
                continue;
            if (CoolDownManager.RemainingTimePercent("mod_"+player.modifiers[i].ToString()) > 0)
            {
                if (firstModifier == 0)
                {
                    if (player.modifiers[i].isBuff())
                        firstModifier = 1;
                    else
                        firstModifier = -1;
                }
                Rect rect;
                if ((player.modifiers[i].isBuff() && firstModifier == 1) || (!player.modifiers[i].isBuff() && firstModifier == -1))
                {
                    rect = firstLine[firstLineCount].GetRect();
                    firstLineCount++;
                } else
                {
                    rect = secondLine[secondLineCount].GetRect();
                    secondLineCount++;
                }

                if(modTextures.Contains(player.modifiers[i]))
                    GUI.DrawTexture(rect, modTextures[player.modifiers[i]], ScaleMode.ScaleToFit);
                
                
            }
            else
            {
                player.RemoveModifier(player.modifiers[i]);
                i = 0;
            }
        }

        //print("1stLine = " + firstLineCount + "\t2ndLine = " + secondLineCount + "\tfirstModifier = " + firstModifier);

        if (firstModifier == 1)
        {
            if (firstLineCount == 0)
            {
                if (secondLineCount == 0)
                    firstModifier = 0;
                else
                {
                    //print("else");
                    firstModifier = -1;
                }
            }
        }
        else if (firstModifier == -1)
        {
            if (firstLineCount == 0)
            {
                if (secondLineCount == 0)
                    firstModifier = 0;
                else
                    firstModifier = 1;
            }
        }


    }

}
