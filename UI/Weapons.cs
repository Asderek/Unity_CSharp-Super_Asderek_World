using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;
using System;

public class Weapons : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponSettings
    {
        public Texture2D elementActivated;
        public Texture2D elementDeactivated;
        public Texture2D weaponTexture;
        public Texture2D animalDeactivated;
        public Texture2D animalActivated;
        public GameObject normalAttack;
        public GameObject specialAttack;
        public bool obtained;
        [HideInInspector]
        public Commandments.Element id;
    }

    [System.Serializable]
    public class WeaponsSettings : SerializableDictionary<Commandments.Element, WeaponSettings> { }

    private UIManager manager;
    public WeaponsSettings settings;
    private Commandments.Weapon selectedWeapon;

    private float ultPercent = 0f;
    
    public SmallWeaponMenu smallMenu;
    private bool showSmallMenu;


    void Start()
    {
        bool startWeapon = false;
        for (Commandments.Element element = Commandments.Element.EARTH; element < Commandments.Element.NEUTRAL; element++)
        {
            WeaponSettings aux = settings[element];
            aux.id = element;
            settings[element] = aux;
            if ((!startWeapon) && (aux.obtained))
            {
                startWeapon = true;
                selectedWeapon = element.toWeapon();
            }
        }

        manager = UIManager.GetInstance();
        
        smallMenu.Init(settings.Count, selectedWeapon);
        showSmallMenu = false;
    }


    public void UpdateOnPlay()
    {
        if (showSmallMenu)
        {
            smallMenu.Update(selectedWeapon.toInt());
            smallMenu.menuArea.AdjustToSquare();
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.L1, this))
        {
            smallMenu.menuArea.Grow(true);
            showSmallMenu = true;
            manager.Slow(0.1f);
        }

        if (ButtonManager.GetUp(ButtonManager.ButtonID.L1, this))
        {
            smallMenu.menuArea.Reduce(() =>
            {
                showSmallMenu = false;
                manager.Slow(1f);
                SetSelectedWeapon((Commandments.Weapon)smallMenu.menuIndex.Get());
                
            });
        }

        //if (Input.GetButtonDown("ADDWEAPON"))
        //{
        //    Commandments.ZodiacAnimal animal = (Commandments.ZodiacAnimal)Random.Range(1, 12);
        //    manager.ActivateWeapon(animal);
        //    weaponFullSize.StartGrowing();
        //    weaponFullSize.StartMoveFrom(new Vector2(0, 1));
        //}



    }

    public void SetSelectedWeapon(Commandments.Weapon weapon)
    {
        smallMenu.ChangeWeaponIndex(weapon);
        selectedWeapon = weapon;
    }

    public void UpdateOnPause()
    {
        smallMenu.Update(selectedWeapon.toInt());
        smallMenu.menuArea.AdjustToSquare();
    }
    
    public void DisplayOnPlay()
    {
        if (showSmallMenu)
        {
            smallMenu.Draw();
        }
    }
    
    public void DisplayOnPause()
    {
        smallMenu.Draw();
    }
    
    public Commandments.Weapon getSelectedWeapon()
    {
        return selectedWeapon;
    }

    public WeaponSettings GetWeapon(Commandments.Weapon weapon)
    {
        return settings[weapon.toElement()];
    }

    public WeaponSettings getCurrentWeapon()
    {
        return settings[selectedWeapon.toElement()];
    }


    public void ChangeState(UIManager.Mode mode)
    {
        smallMenu.ChangeState(mode);
        showSmallMenu = false;
        selectedWeapon = (Commandments.Weapon)smallMenu.menuIndex.Get();
    }
    
    public void ChangeUlt(float ratio)
    {
        ultPercent = ratio;
    }
    
    public float GetUlt()
    {
        return ultPercent;
    }

    public void ObtainWeapon(Commandments.Element element, bool obtain = true)
    {
        WeaponSettings aux = settings[element];
        aux.obtained = obtain;
        settings[element] = aux;
    }

    public void ApplyJson(SaveJson.WeaponsInfo[] save)
    {
        if (save == null)
        {
            for (Commandments.Element element = Commandments.Element.EARTH; element < Commandments.Element.NEUTRAL; element++)
            {
                ObtainWeapon(element, element == Commandments.Element.DEATH);
            }

        }
        else
        {
            foreach (SaveJson.WeaponsInfo weapon in save)
            {
                ObtainWeapon(weapon.id.toElement(), weapon.obtained);
            }
        }


    }

    public List<SaveJson.WeaponsInfo> SaveJson()
    {
        List<SaveJson.WeaponsInfo> saves = new List<SaveJson.WeaponsInfo>();
        for (Commandments.Element element = Commandments.Element.EARTH; element < Commandments.Element.NEUTRAL; element++)
        {
            SaveJson.WeaponsInfo weapon = new SaveJson.WeaponsInfo();
            weapon.id = (Commandments.Weapon)element.toWeapon();
            weapon.obtained = settings[element].obtained;
            saves.Add(weapon);
        }
        return saves;
    }

    #region NewWeapon



    //public void UpdateOnNewWeapon()
    //{
    //    weaponFullSize.Update();

    //    if (lastActivateTimeControl == 0)
    //    {
    //        manager.currentCamera.StartBouncing(0.02f, 0.9f / newWeaponSpeed);
    //    }

    //    if (lastActivateTimeControl > 1f && !alreadyDone)
    //    {
    //        alreadyDone = true;
    //        manager.currentCamera.StartBouncing(0.2f, 0.5f / newWeaponSpeed, true);
    //    }

    //    if (lastActivateTimeControl > 1.8f)
    //    {
    //        WeaponSettings aux = settings[lastActiveWeapon.toElement()];
    //        aux.obtained = true;
    //        settings[lastActiveWeapon.toElement()] = aux;

    //        manager.ChangeMode(UIManager.Mode.NORMAL);
    //        selectedWeapon = lastActiveWeapon.toInt();
    //    }

    //    lastActivateTimeControl += newWeaponSpeed;
    //}


    //public void DisplayOnNewWeapon()
    //{

    //    radiusFull = Screen.width * weaponFullSize.SizeX * 0.41875f;
    //    WeaponBallSize = Screen.width * weaponFullSize.SizeX * 0.1125f;
    //    screenBaseFull = new Vector2(weaponFullSize.GetPos().x - WeaponBallSize / 2, weaponFullSize.GetPos().y - WeaponBallSize / 2);
    //    unitaryVectorFull = new Vector2(0, -radiusFull);

    //    Vector2 startPosition = new Vector2(Screen.width *0.5f, Screen.height * 0.5f);
    //    Vector2 returnPosition = new Vector2(Screen.width * 0.8f, Screen.height * 0.5f);

    //    Vector2 angle;
    //    angle = unitaryVectorFull;
    //    angle = Quaternion.Euler(0, 0, lastActiveWeapon.toInt() * 360 / weaponsSettings.Length) * angle;

    //    Vector2 destinyPosition = new Vector2(screenBaseFull.x + angle.x, screenBaseFull.y + angle.y);

    //    DisplayOnPause();

    //    if (lastActivateTimeControl < goAwayTime)
    //    {
    //        Vector2 origin = Vector2.Lerp(startPosition, returnPosition, 0.5f);
    //        Vector2 vect = startPosition - origin;
    //        Vector2 pos = origin + (Vector2)(Quaternion.Euler(0, 0, -180 * lastActivateTimeControl / goAwayTime) * vect);
    //        DrawLastActiveWeapon(pos);
    //    }
    //    else
    //    {

    //        Vector2 pos = Vector2.Lerp(returnPosition, destinyPosition, (lastActivateTimeControl - goAwayTime) / (1 - goAwayTime));

    //        if (lastActivateTimeControl <= 1)
    //        {
    //            float newLast = lastActivateTimeControl - goAwayTime;
    //            float newGo = 1 - goAwayTime;

    //            pos = pos + ((Vector2)((Quaternion.Euler(0, 0, 90) * (destinyPosition - returnPosition).normalized))) 
    //                    * (
    //                        (((-(newLast - newGo / 2f) * (newLast - newGo / 2f) + (newGo / 2f) * (newGo / 2f))) / 0.09f)
    //                    ) * verticalIntensity * Screen.height;
    //        }
    //        DrawLastActiveWeapon(pos);

    //    }


    //}

    //public void ActiveNewWeapon(Commandments.Weapon weap)
    //{
    //    alreadyDone = false;
    //    lastActivateTimeControl = 0f;
    //    lastActiveWeapon = weap;
    //    manager.ChangeMode(UIManager.Mode.NEW_WEAPON);
    //}

    #endregion
    
    #region WarningElementArea

    //public void ActiveWarning(bool state)
    //{
    //    if (state != blink)
    //    {
    //        whitenessLevels = 0;
    //        blink = state;
    //    }
    //}


    #endregion
    
    //void Update()
    //{
    //    /*
    //    unitaryVectorFull = new Vector2(0, -radiusFull);

    //    whitenessLevels += colorSpeed;
    //    if (whitenessLevels < 0 || whitenessLevels > 0.35)
    //    {
    //        colorSpeed *= -1;
    //    }
    //    */
    //    if (count > 0)
    //        count--;
    //}

    //public void DrawSpecialIndicator()
    //{
    //    animalTextureOrigin.x = ultPos.x * Screen.width;
    //    animalTextureOrigin.y = ultPos.y * Screen.height;

    //    GUI.DrawTexture(new Rect(animalTextureOrigin.x, animalTextureOrigin.y, Screen.height * miniSize * ultRatio, Screen.height * miniSize * ultRatio), weaponsSettings[selectedWeapon].animalDeactivated, ScaleMode.StretchToFill, true, 0);
    //    Rect rectSpace = new Rect(animalTextureOrigin.x, animalTextureOrigin.y + (1 - percent) * Screen.height * miniSize * ultRatio, Screen.height * miniSize * ultRatio, percent * Screen.height * miniSize * ultRatio);
    //    Rect rectText = new Rect(0, 0, 1, percent);
    //    GUI.DrawTextureWithTexCoords(rectSpace, weaponsSettings[selectedWeapon].animalActivated, rectText, true);
    //}

    //public void setSelectedWeapon(int selection)
    //{
    //    if (Mathf.Abs(selection) < weaponsSettings.Length)
    //    {
    //        selectedWeapon = Mathf.Abs(selection);
    //        //manager.setActiveWeapon((Commandments.Weapon)selectedWeapon);
    //    }

    //}


    //public void DrawLastActiveWeapon(Vector2 position)
    //{
    //    GUI.DrawTexture(new Rect(position.x, position.y, WeaponBallSize, WeaponBallSize), weaponsSettings[lastActiveWeapon.toInt()].weaponsActivatedFull, ScaleMode.StretchToFill, true, 0);
    //}

    //private void DrawWeaponsWheelMiniShader()
    //{
    //    //GUI.DrawTexture(new Rect(0, (Screen.height) - shaderMini.height, shaderMini.width, shaderMini.height), shaderMini, ScaleMode.StretchToFill, true, 0);
    //    GUI.DrawTexture(new Rect(0, Screen.height * (1 - miniSize), Screen.height * miniSize, Screen.height * miniSize), shaderMini, ScaleMode.StretchToFill, true, 0);
    //}

    //private void DrawWeaponWheelFullShader()
    //{
    //    /*Shader = */
    //    GUIUtility.RotateAroundPivot(shaderAngle, shaderPivot);
    //    GUI.DrawTexture(new Rect((Screen.width / 2) - WeaponTabletHeight / 2, (Screen.height / 2) - WeaponTabletHeight / 2, WeaponTabletHeight, WeaponTabletHeight), shaderWheel[shaderIndex], ScaleMode.StretchToFill, true, 0);
    //    GUIUtility.RotateAroundPivot(-shaderAngle, shaderPivot);
    //}

    //private void DrawWeaponsMini()
    //{
    //    Vector2 angle1, angle2, angle3, angle4;

    //    float percent = ((Time.time - lastTime) / cooldown);

    //    angle1 = Quaternion.Euler(0, 0, startAngle + (0 + rotate * percent) * stepAngle) * unitaryVectorMini;
    //    angle2 = Quaternion.Euler(0, 0, startAngle + (1 + rotate * percent) * stepAngle) * unitaryVectorMini;
    //    angle3 = Quaternion.Euler(0, 0, startAngle + (2 + rotate * percent) * stepAngle) * unitaryVectorMini;
    //    angle4 = Quaternion.Euler(0, 0, startAngle + (((-2 * rotate) + 1) + rotate * percent) * stepAngle) * unitaryVectorMini;

    //    int lastIndex = GetNextActive(false, selectedWeapon);
    //    int nextIndex = GetNextActive(true, selectedWeapon);

    //    if (weaponsSettings[lastIndex].obtained)
    //        GUI.DrawTexture(new Rect(screenBaseMini.x + angle1.x, screenBaseMini.y + angle1.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), weaponsSettings[lastIndex].weaponsActivatedMini, ScaleMode.StretchToFill, true, 0);
    //    else
    //        GUI.DrawTexture(new Rect(screenBaseMini.x + angle1.x, screenBaseMini.y + angle1.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), noWeaponMini, ScaleMode.StretchToFill, true, 0);

    //    if (weaponsSettings[selectedWeapon].obtained)
    //    {
    //        if (blink == true)
    //        {
    //            Color[] colorArray = new List<Color>(weaponsSettings[selectedWeapon].weaponsActivatedMini.GetPixels()).ToArray();
    //            auxText = new Texture2D(weaponsSettings[selectedWeapon].weaponsActivatedMini.width, weaponsSettings[selectedWeapon].weaponsActivatedMini.height);

    //            for (int i = 0; i < colorArray.Length; i++)
    //            {
    //                colorArray[i].r += whitenessLevels;
    //                colorArray[i].b += whitenessLevels;
    //                colorArray[i].g += whitenessLevels;
    //            }
    //            auxText.SetPixels(colorArray);
    //            auxText.Apply();
    //            GUI.DrawTexture(new Rect(screenBaseMini.x + angle2.x, screenBaseMini.y + angle2.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), auxText, ScaleMode.StretchToFill, true, 0);
    //        }
    //        else
    //            GUI.DrawTexture(new Rect(screenBaseMini.x + angle2.x, screenBaseMini.y + angle2.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), weaponsSettings[selectedWeapon].weaponsActivatedMini, ScaleMode.StretchToFill, true, 0);
    //    }
    //    else
    //        GUI.DrawTexture(new Rect(screenBaseMini.x + angle2.x, screenBaseMini.y + angle2.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), noWeaponMini, ScaleMode.StretchToFill, true, 0);

    //    if (weaponsSettings[nextIndex].obtained)
    //        GUI.DrawTexture(new Rect(screenBaseMini.x + angle3.x, screenBaseMini.y + angle3.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), weaponsSettings[nextIndex].weaponsActivatedMini, ScaleMode.StretchToFill, true, 0);
    //    else
    //        GUI.DrawTexture(new Rect(screenBaseMini.x + angle3.x, screenBaseMini.y + angle3.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), noWeaponMini, ScaleMode.StretchToFill, true, 0);






    //    if (rotate == 1)
    //    {
    //        int otherIndex = GetNextActive(false, lastIndex);

    //        if (weaponsSettings[otherIndex].obtained)
    //            GUI.DrawTexture(new Rect(screenBaseMini.x + angle4.x, screenBaseMini.y + angle4.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), weaponsSettings[otherIndex].weaponsActivatedMini, ScaleMode.StretchToFill, true, 0);
    //        else
    //            GUI.DrawTexture(new Rect(screenBaseMini.x + angle4.x, screenBaseMini.y + angle4.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), noWeaponMini, ScaleMode.StretchToFill, true, 0);



    //    }
    //    else if (rotate == -1)
    //    {
    //        int otherIndex = GetNextActive(true, nextIndex);

    //        if (weaponsSettings[otherIndex].obtained)
    //            GUI.DrawTexture(new Rect(screenBaseMini.x + angle4.x, screenBaseMini.y + angle4.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), weaponsSettings[otherIndex].weaponsActivatedMini, ScaleMode.StretchToFill, true, 0);
    //        else
    //            GUI.DrawTexture(new Rect(screenBaseMini.x + angle4.x, screenBaseMini.y + angle4.y, Screen.height * miniSize * ballRatio, Screen.height * miniSize * ballRatio), noWeaponMini, ScaleMode.StretchToFill, true, 0);
    //    }





    //}

    //private void DrawWeaponsFull()
    //{
    //    Vector2 angle;

    //    angle = unitaryVectorFull;


    //    foreach (WeaponSettings current in weaponsSettings)
    //    {
    //        if (!current.obtained)
    //        {
    //            GUI.DrawTexture(new Rect(screenBaseFull.x + angle.x, screenBaseFull.y + angle.y, WeaponBallSize, WeaponBallSize), noWeapon, ScaleMode.StretchToFill, true, 0);
    //        }
    //        else
    //        {
    //            if (current.Equals(weaponsSettings[selectedWeapon]))
    //                GUI.DrawTexture(new Rect(screenBaseFull.x + angle.x, screenBaseFull.y + angle.y, WeaponBallSize, WeaponBallSize), current.weaponsActivatedFull, ScaleMode.StretchToFill, true, 0);
    //            else
    //                GUI.DrawTexture(new Rect(screenBaseFull.x + angle.x, screenBaseFull.y + angle.y, WeaponBallSize, WeaponBallSize), current.weaponsDeactivated, ScaleMode.StretchToFill, true, 0);
    //        }
    //        angle = Quaternion.Euler(0, 0, 360 / weaponsSettings.Length) * angle;
    //    }


    //}

    //protected void DrawWeaponWheelFull()
    //{
    //    /*WeaponWheel = */
    //    GUI.DrawTexture(new Rect((Screen.width / 2) - WeaponTabletHeight / 2, (Screen.height / 2) - WeaponTabletHeight / 2, WeaponTabletHeight, WeaponTabletHeight), weaponWheelFull, ScaleMode.StretchToFill, true, 0);
    //}

    //protected void DrawWeaponWheelMini()
    //{
    //    GUI.DrawTexture(new Rect(0, Screen.height * (1 - miniSize), Screen.height * miniSize, Screen.height * miniSize), weaponWheelMini, ScaleMode.StretchToFill, true, 0);
    //}

    //public void ChangeWeaponWheel(bool isPause)
    //{
    //    this.isPause = isPause;
    //}

    //public int GetNextActive(bool clockwise, int initialIndex)
    //{

    //    int index = initialIndex;
    //    do
    //    {
    //        if (clockwise)
    //            index++;
    //        else
    //            index--;

    //        if (index < 0)
    //            index += weaponsSettings.Length;

    //        if (index >= weaponsSettings.Length)
    //            index -= weaponsSettings.Length;

    //    } while (!weaponsSettings[index].obtained && index != initialIndex);

    //    return index;
    //}

    //// Use this for initialization
    //void Start()
    //{

    //    lastActiveWeapon = Commandments.Weapon.HAMMER;

    //    float verticalSize = 0.75f;
    //    float horizontalSize = 0.35f;
    //    WeaponTabletHeight = Mathf.Min(Screen.height * verticalSize, Screen.width * horizontalSize);
    //    WeaponBallSize = WeaponTabletHeight * 0.1125f;
    //    radiusFull = WeaponTabletHeight * 0.41875f;

    //    manager = UIManager.GetInstance();

    //    shaderPivot = new Vector2(Screen.width / 2, Screen.height / 2);
    //    selectedWeapon = GetNextActive(true, 4);

    //    //manager.setActiveWeapon((Commandments.Weapon)selectedWeapon);

    //    shaderIndex = selectedWeapon % shaderWheel.Length;
    //    shaderAngle = (360 / (shaderWheel.Length + 1)) * (selectedWeapon / shaderWheel.Length);

    //    lastTime = -cooldown;

    //    screenBaseFull = new Vector2(Screen.width / 2 - WeaponBallSize / 2, Screen.height / 2 - WeaponBallSize / 2);
    //    unitaryVectorFull = new Vector2(0, -radiusFull);

    //    // Change resolution should recalcule these variables
    //    screenBaseMini = new Vector2(-Screen.height * miniSize * ballRatio / 2, Screen.height * (1 - miniSize * ballRatio / 2));
    //    unitaryVectorMini = new Vector2(0, -Screen.height * miniSize * radiusMini);
    //    animalTextureOrigin = new Vector2(miniSize * -12 / 0.27f, Screen.height * (1 - 0.151f * miniSize / 0.27f));//Screen.height *( 1 - miniSize*ultRatio * 0.28f ));
    //                                                                                                               //screenBaseMini = new Vector2(-weaponsSettings[selectedWeapon].weaponsActivatedMini.width / 2, Screen.height - weaponsSettings[selectedWeapon].weaponsActivatedMini.height / 2);
    //                                                                                                               //print((("--" + Screen.height + " -> " + weaponsSettings[0].animalDeactivated.height*0.28f);
    //                                                                                                               //animalTextureOrigin = new Vector2(-12f, Screen.height - weaponsSettings[0].animalDeactivated.height * 0.28f);

    //    if (Screen.width * weaponFullSize.SizeX > Screen.height * weaponFullSize.SizeY)
    //    {
    //        weaponFullSize.SizeX = ((float)weaponFullSize.SizeY * Screen.height) / ((float)Screen.width);
    //    }
    //    else if (Screen.width * weaponFullSize.SizeX < Screen.height * weaponFullSize.SizeY)
    //    {
    //        weaponFullSize.SizeY = ((float)weaponFullSize.SizeX * Screen.width) / ((float)Screen.height);
    //    }


    //    smallMenu.Init(weaponsSettings, selectedWeapon);
    //    showSmallMenu = false;
    //}


}
