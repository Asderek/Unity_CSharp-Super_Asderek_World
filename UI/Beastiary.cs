using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beastiary : MonoBehaviour
{

    UIManager manager;
    public BasicMenuBackground starsBackground;

    public enum Status { Unknown, Found, Known };
    public float raioHori;
    public float raioVert;
    private int startCount;

    [System.Serializable]
    public enum YokaiID
    {
        Fire_Furarubi = 104,
        Fire_Wanyudo = 105,
        Fire_Kasha = 106,
        Fire_Basan = 107,
        Fire_Garuda = 0,
        Fire_Hanzaki = 1,
        Fire_Nekomata = 2,
        Fire_KiyoHime = 3,
        Fire_Sogenbi = 4
    }

    [System.Serializable]
    public struct Yokai
    {
        public Texture2D sprite;
        public string name;
        public string description;
        public Texture2D script;
        public Status status;
    }
    private int i;
    public Yokai[] yokais;
    public Texture2D background;
    public Texture2D scriptBackground;

    public float lastZoomCommand;
    public float step = 0.05f;

    private int selected = 81;

    public Texture2D knownYokai;
    public Texture2D unknownYokai;

    [System.Serializable]
    public struct YokaiGroupImage
    {
        public int rank;
        public Texture2D imagem;
        [HideInInspector]
        public Texture2D black;
    }

    [System.Serializable]
    public struct YokaiGroup
    {
        public Texture2D backgroundScalable;
        public Texture2D backgroundFixed;
        public YokaiGroupImage[] images;
    }

    [System.Serializable]
    public struct Boss
    {
        public Texture2D bead;
        public Texture2D picture;
        public YokaiGroup folks;
    }

    public Boss[] bosses;
    public Texture2D unknownBoss;
    public Texture2D cord;
    public Texture2D test;
    public Texture2D interrogation;
    private List<Rect> rects;
    private List<Rect> rectsGuardians;
    private List<Rect> rectsZoomed;


    float a, b, P, c, a2, b2, c2, e, lastDelta;
    public float deltaOffSet = 0;
    private float constDelta;
    private bool giveShit = false;
    private Texture2D auxText;
    private float whitenessLevels;
    private float colorSpeed = 0.015f;
    private Texture2D unknownAux;
    private Texture2D knownAux;

    public Texture2D sideBarSelected;
    public Texture2D sideBarNotSelected;

    public float ratio;
    private float vertRatio;
    private float horRatio;
    public int layer = 2;

    public int offset = 0;

    private Color[] colorArray;
    private List<Color> auxList;

    public bool isZoommed = true;
    private bool tabSelected = true;

    private string label1 = "Information";
    private string label2 = "Script";
    private float linePosition = 0f;
    public float lineStep= 0.02f;

    private Rect pos;
    public float scrollStep = 0.02f;
    public Vector2 vetorTeste;
    public Texture2D scrollBackground;

    private ButtonManager buttonManager;

    void Start()
    {

        manager = UIManager.GetInstance();
        buttonManager = ButtonManager.GetInstance();

        if (isZoommed)
        {
            lastZoomCommand = 1;
            step = Mathf.Abs(step);
        }
        else
        {
            lastZoomCommand = 0;
            step = -Mathf.Abs(step);
        }


        auxList = new List<Color>();

        unknownAux = new Texture2D(unknownYokai.width, unknownYokai.height);
        knownAux = new Texture2D(knownYokai.width, knownYokai.height);
        auxText = new Texture2D(bosses[0].bead.width, bosses[0].bead.height);

        ratio = Screen.height / 638f * 32f;
        //print((("ScreenHeight = "+Screen.height);

        raioHori = 0.25f * Screen.width;
        raioVert = 0.35f * Screen.height;

        rectsZoomed = new List<Rect>();
        rectsZoomed.Add(new Rect(0, -Screen.height / 2, ratio * 4, ratio * 4));

        float alfa = ratio * 3.0f / (Screen.height / 2.0f);
        int i = 0;

        Vector2 vector = new Vector2(0, -Screen.height / 2);
        vector = Quaternion.Euler(0, 0, alfa * Mathf.Rad2Deg) * vector;

        rectsZoomed.Add(new Rect(-vector.x, vector.y, ratio * 2, ratio * 2));

        alfa = ratio * 2.0f / (Screen.height / 2.0f);

        for (i = 0; i < 3; i++)
        {
            vector = Quaternion.Euler(0, 0, alfa * Mathf.Rad2Deg) * vector;
            rectsZoomed.Add(new Rect(-vector.x, vector.y, ratio * 2, ratio * 2));
        }

        rectsZoomed.Reverse(0, rectsZoomed.Count);

        for (i = rectsZoomed.Count - 2; i >= 0; i--)
        {
            rectsZoomed.Add(new Rect(-rectsZoomed[i].x, rectsZoomed[i].y, rectsZoomed[i].width, rectsZoomed[i].height));
        }

        for (i = 0; i < rectsZoomed.Count; i++)
        {
            rectsZoomed[i] = new Rect(rectsZoomed[i].x + (Screen.width / 3) - rectsZoomed[i].width / 2, rectsZoomed[i].y + Screen.height - rectsZoomed[i].height / 2, rectsZoomed[i].width, rectsZoomed[i].height);
        }


        rectsGuardians = new List<Rect>();

        rectsGuardians.Add(new Rect(Screen.width / 3 - ratio * 1.25f / 2 - 0.3f * raioHori, Screen.height / 2 - (ratio * 1.25f / 2), ratio * 1.25f, ratio * 1.25f));
        rectsGuardians.Add(new Rect(Screen.width / 3 - ratio * 1.25f / 2, Screen.height / 2 + 0.4f * raioVert - (ratio * 1.25f / 2), ratio * 1.25f, ratio * 1.25f));
        rectsGuardians.Add(new Rect(Screen.width / 3 - ratio * 1.25f / 2 + 0.3f * raioHori, Screen.height / 2 - (ratio * 1.25f / 2), ratio * 1.25f, ratio * 1.25f));
        rectsGuardians.Add(new Rect(Screen.width / 3 - ratio * 1.25f / 2, Screen.height / 2 - 0.4f * raioVert - (ratio * 1.25f / 2), ratio * 1.25f, ratio * 1.25f));


        {
            a = raioHori;
            b = raioVert;

            //print((("a " + a + " b " + b);

            a2 = a * a;
            b2 = b * b;
            c2 = c * c;
            e = c / a;
            lastDelta = 1.34f;

            c = Mathf.Sqrt(Mathf.Abs(a2 - b2));

            P = 2 * Mathf.PI * Mathf.Sqrt((a2 + b2) / 2) / 120;

            //print((("Perimetro  = " + P);
        }


        rects = new List<Rect>();

        float x0 = -a;
        float y0 = 0;
        int cont = 0;

        while (cont < 10)
        {
            float dx, dy;
            dx = 0;
            if (cont % 9 == 0)
                rects.Add(new Rect(x0, y0, ratio, ratio));
            else
                rects.Add(new Rect(x0, y0, ratio / 2, ratio / 2));

            if (!giveShit)
            {
                if (cont % 9 == 0)
                {
                    P += P / 2;
                }
                if (cont == 8)
                {
                    P += P / 4;
                }
                //print((("before x0 = " + x0);
                dx = delta(x0, y0);
                x0 = x0 + dx;
                if (x0 > a)
                    x0 = a;
                // //print((("after x0 = " + x0);
                if (cont % 9 == 0)
                {
                    P -= P / 3;
                }
                if (cont == 8)
                {
                    P -= P * 4 / 5;
                }

            }
            else
                x0 += constDelta;

            dy = y0;
            y0 = b * Mathf.Sqrt(1.0f - ((x0 * x0) / (a * a)));
            dy -= y0;

            //print((("dx " + dx + " dy " + dy + "andei " + Mathf.Sqrt((dx * dx) + dy * dy));
            if (cont > 60)
            {
                y0 = -y0;
            }
            cont++;
        }

        foreach (Rect r in rects)
        {
            //print(((r.x + r.y);
        }

        constDelta = (0 - x0) / 19 * 0.9f;
        cont = 1;

        for (i = 0; i < 4; i++)
        {
            x0 += constDelta;
            y0 = b * Mathf.Sqrt(1.0f - ((x0 * x0) / (a * a)));
            rects.Add(new Rect(x0, y0, ratio / 2, ratio / 2));
            cont++;
        }

        constDelta = (0 - x0) / 15.5f;


        while (x0 < -1)
        {
            x0 += constDelta;
            //print((("x0 " + x0);
            y0 = b * Mathf.Sqrt(1.0f - ((x0 * x0) / (a * a)));

            if (cont % 9 == 0)
                rects.Add(new Rect(x0, y0, ratio, ratio));
            else
                rects.Add(new Rect(x0, y0, ratio / 2, ratio / 2));

            if (cont % 8 == 0)
                constDelta *= 1.5f;
            if (cont % 10 == 0)
            {
                constDelta /= 1.5f;
                cont = 1;
            }

            cont++;
        }

        //print((("Last " + rects[rects.Count - 1].x + "-" + rects[rects.Count - 1].y);
        //float err = rects[rects.Count - 1].x;
        //print((("Count " + rects.Count);
        for (i = rects.Count - 2; i >= 0; i--)
        {
            rects.Add(new Rect(-rects[i].x, rects[i].y, rects[i].width, rects[i].height));
        }

        //print((("Count " + rects.Count);
        for (i = rects.Count - 2; i > 0; i--)
        {
            rects.Add(new Rect(rects[i].x, -rects[i].y, rects[i].width, rects[i].height));
        }

        for (i = 0; i < rects.Count; i++)
        {
            rects[i] = new Rect(rects[i].x + (Screen.width / 3) - rects[i].width / 2, rects[i].y + Screen.height / 2 - rects[i].height / 2, rects[i].width, rects[i].height);
        }


        for (i=0; i < bosses.Length; i++) { 
            for (int j = 0; j < bosses[i].folks.images.Length; j++)
            {
                bosses[i].folks.images[j].black = new Texture2D(bosses[i].folks.images[j].imagem.width, bosses[i].folks.images[j].imagem.height);
                List<Color> aux = new List<Color>();
                Color[] colorAux;
                aux.AddRange(bosses[i].folks.images[j].imagem.GetPixels());
                colorAux = aux.ToArray();

                for (int k = 0; k < colorAux.Length; k++)
                {
                    colorAux[k].r = 0;
                    colorAux[k].b = 0;
                    colorAux[k].g = 0;
                }
                bosses[i].folks.images[j].black.SetPixels(colorAux);
                bosses[i].folks.images[j].black.Apply();
            }
        }
    }
    
    void Update()
    {
        startCount += 1;

        if (manager.mode != UIManager.Mode.BESTIARY)
            return;

        if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
        {

                if (layer == 2)
                {
                    if (!isZoommed)
                    {
                        lastZoomCommand = 0;
                        isZoommed = true;
                        offset = 0;
                        step = +Mathf.Abs(step);
                    }
                }
        }
        if (ButtonManager.GetDown(ButtonManager.ButtonID.CIRCLE, this))
        {

            if (isZoommed == false)
            {
                if(startCount > 40)
                    manager.ChangeMode(UIManager.Mode.NORMAL);
            }
            else
            {
                step = -Mathf.Abs(step);
                isZoommed = false;
            }
        }

        lastZoomCommand += step;
        
        if (lastZoomCommand > 1)
        {
                lastZoomCommand = 1;
        }
        else if (lastZoomCommand < 0)
        {
            lastZoomCommand = 0;
        }

        if (!isZoommed)
        {
            if (ButtonManager.GetDown(ButtonManager.ButtonID.L1, this))
            {
                whitenessLevels = 0;
                if (layer == 2)
                    selected += 9;
                else
                    selected += 27;

                if (selected >= 108)
                {
                    selected = 0;
                }
                linePosition = 0;
                vetorTeste.y = 0;
            }
            else if (ButtonManager.GetDown(ButtonManager.ButtonID.R1, this))
            {
                whitenessLevels = 0;
                if (layer == 2)
                    selected -= 9;
                else
                    selected -= 27;
                if (selected < 0)
                {
                    if (layer == 2)
                        selected = 99;
                    else
                        selected = 81;
                }
                linePosition = 0;
                vetorTeste.y = 0;
            }


            if (ButtonManager.Get(ButtonManager.ButtonID.L_LEFT))
            {
                if ((selected < 68 && selected > 41) && (layer == 2))
                {
                    layer = 1;
                    selected = 54;
                }
                else if ((selected < 14 || selected > 95) && (layer == 1))
                {
                    layer = 2;
                }
                linePosition = 0;
                vetorTeste.y = 0;

            }
            else if (ButtonManager.Get(ButtonManager.ButtonID.L_RIGHT))
            {
                if ((selected < 14 || selected > 95) && (layer == 2))
                {
                    layer = 1;
                    selected = 0;
                }
                else if ((selected < 68 && selected > 41) && (layer == 1))
                {
                    layer = 2;
                }
                linePosition = 0;
                vetorTeste.y = 0;

            }



            if (ButtonManager.Get(ButtonManager.ButtonID.L_UP))
            {
                if ((selected < 41 && selected > 14) && (layer == 2))
                {
                    layer = 1;
                    selected = 27;
                }
                else if ((selected < 95 && selected > 68) && (layer == 1))
                {
                    layer = 2;
                }
            }
            else if (ButtonManager.Get(ButtonManager.ButtonID.L_DOWN))
            {
                if ((selected < 95 && selected > 68) && (layer == 2))
                {
                    layer = 1;
                    selected = 81;
                }
                else if ((selected < 41 && selected > 14) && (layer == 1))
                {
                    layer = 2;
                }

            }


        }
        else 
        {
            
            if (Input.GetAxisRaw("L_HORIZONTAL") < 0)
            {
                tabSelected = false;
                linePosition = 0;
                vetorTeste.y = 0;
            }
            else if (Input.GetAxisRaw("L_HORIZONTAL") > 0)
            {
                tabSelected = true;
                linePosition = 0;
                vetorTeste.y = 0;
            }

            if (Input.GetAxisRaw("L_VERTICAL") < 0)
            {
                if (tabSelected)
                    linePosition += lineStep;
                else
                    vetorTeste.y += scrollStep;
            }
            else if (Input.GetAxisRaw("L_VERTICAL") > 0)
            {
                if (tabSelected)
                    linePosition -= lineStep;
                else
                    vetorTeste.y -= scrollStep;
            }

            if (linePosition < 0f)
            {
                linePosition = 0;
            }
            else if (linePosition > 0.5f){
                linePosition = 0.5f;
            }


            if (Input.GetButtonDown("L1"))
            {
                if ((selected > 50) || (selected < 4))
                    offset++;
                else
                    offset--;

                if (offset < -4)
                    offset = -4;
                else if (offset > 4)
                    offset = 4;
                else 
                    whitenessLevels = 0;

                linePosition = 0;
                vetorTeste.y = 0;
            }
            else if (Input.GetButtonDown("R1"))
            {
                if ((selected > 50) || (selected < 4))
                    offset--;
                else
                    offset++;

                if (offset < -4)
                    offset = -4;
                else if (offset > 4)
                    offset = 4;
                else
                    whitenessLevels = 0;

                linePosition = 0;
                vetorTeste.y = 0;
            }
        }
        whitenessLevels += colorSpeed;
        if (whitenessLevels < 0 || whitenessLevels > 0.35)
        {
            colorSpeed *= -1;
        }
    }
   
    private float delta(float x0, float y0)
    {
        //print((("Ponto ["+asd+"] x0 " + x0 + " - y0 " + y0 + "     ==   " +( (x0*x0)/(a*a) + (y0*y0)/(b*b)));  
        float c1, c2, c3;
        c1 = 1 - (a2 / b2);
        c2 = 2 * x0;
        float aux = Mathf.Sqrt(P * P - lastDelta * lastDelta);
        if (float.IsNaN(aux))
            aux = P;
        c3 = (a2 / b2) * (2 * y0 * aux + P * P);
        //print((( c1 + "dx^2 + " + c2 +"dx +" +c3 +" = 0" );

        //float ret = (-2 * x0 - Mathf.Sqrt(4 * Mathf.Pow(x0, 2) - 4 * (1 - (a2) / (b2)) * a2 / b2 * (2 * y0 * P + P * P)));
        //print((("D = " + ( - 4 * c1 * c3));
        //print((("dx1 = " + ((-c2 + Mathf.Sqrt(c2 * c2 - 4 * c1 * c3)) / (2 * c1)));
        lastDelta = ((-c2 - Mathf.Sqrt(c2 * c2 - 4 * c1 * c3)) / (2 * c1));
        if (lastDelta > P)
        {
            constDelta = (0 - (x0 - lastDelta)) / (30.0f - deltaOffSet);
            lastDelta = P;
            giveShit = true;
        }
        //print((("dx2 = " + lastDelta );

        deltaOffSet++;
        return lastDelta;
        //ret /= 2 * (1 - a2 / b2);
        //print((("ret = " + ret);
        //return ret;
    }

    public void Display()
    {
        auxList.Clear();
        auxList.AddRange( unknownYokai.GetPixels());
        colorArray = auxList.ToArray();

        for (int j = 0; j < colorArray.Length; j++)
        {
            colorArray[j].r += whitenessLevels;
            colorArray[j].b += whitenessLevels;
            colorArray[j].g += whitenessLevels;
        }
        unknownAux.SetPixels(colorArray);
        unknownAux.Apply();

        auxList.Clear();
        auxList.AddRange(knownYokai.GetPixels());
        colorArray = auxList.ToArray();
        
        for (int j = 0; j < colorArray.Length; j++)
        {
            colorArray[j].r += whitenessLevels;
            colorArray[j].b += whitenessLevels;
            colorArray[j].g += whitenessLevels;
        }
        knownAux.SetPixels(colorArray);
        knownAux.Apply();

        starsBackground.Draw();
        GUI.DrawTexture(new Rect(Screen.width * 2f / 3f, Screen.height/3f, Screen.width/3f, Screen.height*2f/3f), scriptBackground, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);

        DisplayRosarySelectedSegment();

        if (lastZoomCommand==0)
            DisplayRosary();

        DisplaySideBar();


    }

    public void DisplayRosary() {
        
        for (int i = 0; i < Mathf.Min(rects.Count, yokais.Length); i++)
        {
            if (i % 9 == 0)
            {
                if ((i == selected) && (layer == 2))
                {
                
                }
                else
                {
                    if (yokais[i].status == Status.Known)
                        GUI.DrawTexture(rects[i], bosses[i / 9].bead);
                    else
                        GUI.DrawTexture(rects[i], unknownBoss);
                }
            }
            else
            {
                if (((i >= selected - 4 && i <= selected + 4) || (selected == 0 && i >= 104)) && (layer == 2))
                {
                    
                }
                else
                {
                    if (yokais[i].status == Status.Known)
                        GUI.DrawTexture(rects[i], knownYokai);
                    else
                        GUI.DrawTexture(rects[i], unknownYokai);
                }
            }
        }
        vertRatio = 2 * raioVert / cord.height;
        horRatio = 2 * raioHori / cord.width;
        GUI.DrawTexture(new Rect(Screen.width / 3 - (cord.width * horRatio / 2), Screen.height / 2 - (cord.height * vertRatio / 2), cord.width * horRatio, cord.height * vertRatio), cord);

        for (int i = 0; i < rectsGuardians.Count; i++)
        {
            if ((selected / 27 == i) && (layer == 1))
            {
                GUI.DrawTexture(rectsGuardians[i], unknownAux);
            }
            else
                GUI.DrawTexture(rectsGuardians[i], unknownYokai);
        }


    }

    public void DisplayRosarySelectedSegment() {

        if (layer == 2)
        {
            int i = selected / 9;
            if( (offset == 0) || (!isZoommed))
            {

                if (yokais[selected].status == Status.Known)
                {
                    colorArray = new List<Color>(bosses[i].bead.GetPixels()).ToArray();
                }
                else
                {
                    colorArray = new List<Color>(unknownBoss.GetPixels()).ToArray();
                }

                for (int j = 0; j < colorArray.Length; j++)
                {
                    colorArray[j].r += whitenessLevels;
                    colorArray[j].b += whitenessLevels;
                    colorArray[j].g += whitenessLevels;
                }
                auxText.SetPixels(colorArray);
                auxText.Apply();

                DrawZoomed(rects[selected],rectsZoomed[4], auxText);
            }
        else {
            if (yokais[selected].status == Status.Known)
                DrawZoomed(rects[selected], rectsZoomed[4], bosses[i].bead);
            else
            {
                DrawZoomed(rects[selected], rectsZoomed[4], unknownBoss);
            }
         }
            for (i = 0; i < rectsZoomed.Count; i++) {
                if (i != 4) {
                    int aux;
                    bool blink = false;
                    if( (selected > 50) || (selected <4))
                    {
                        aux = selected + 4 - i;
                        blink = offset == (4 - i);
                    }
                    else
                    {
                        aux = selected + i - 4;
                        blink = offset == (i - 4);
                    }
                    if (aux < 0)
                    {
                        aux += 108;
                    }
                    if (yokais[aux].status == Status.Known)
                    {
                        if( (blink)  || (!isZoommed))
                        {
                            DrawZoomed(rects[aux], rectsZoomed[i], knownAux);
                        }
                        else
                        {
                            DrawZoomed(rects[aux], rectsZoomed[i], knownYokai);
                        }
                    }
                    else {
                        if ((blink) || (!isZoommed))
                        {
                            DrawZoomed(rects[aux], rectsZoomed[i], unknownAux);
                        }
                        else
                        {
                            DrawZoomed(rects[aux], rectsZoomed[i], unknownYokai);
                        }
                    }
                }
            }

        }
    }

    public void DisplaySideBar() {

        if (layer == 2) {

            if (lastZoomCommand ==1)
            {
                int aux;
                if( (selected > 50) || (selected <4))
                    aux = selected + offset;
                else
                    aux = selected - offset;

                if (aux < 0)
                    aux += 108;

                if (yokais[aux].sprite!=null && yokais[aux].status == Status.Known)
                    GUI.DrawTexture(new Rect(Screen.width * 0.675f, 0, Screen.width * 0.325f, Screen.height * 0.3912f), yokais[aux].sprite, ScaleMode.ScaleToFit);
                else
                    GUI.DrawTexture(new Rect(Screen.width * 0.675f, 0, Screen.width * 0.325f, Screen.height * 0.3912f), interrogation, ScaleMode.StretchToFill);

                if (tabSelected)
                {
                    if (yokais[aux].script != null && yokais[aux].status == Status.Known)
                    {
                        Rect rectSpace = new Rect(Screen.width * 0.675f, Screen.height * (1- 0.5422f), Screen.width * 0.325f, Screen.height * 0.5422f);
                        float width, height, y;
                        float cropSize = 650;
                        if (yokais[aux].script.width > cropSize)
                        {
                            width = cropSize / yokais[aux].script.width;
                            y = (Screen.height * 0.5422f) * cropSize / (Screen.width * 0.325f);
                        }
                        else
                        {
                            width = 1;
                            y = (Screen.height * 0.5422f) * yokais[aux].script.width / (Screen.width * 0.325f);                           
                        }
                        height = y/yokais[aux].script.height;
                        if (height > 1)
                        {
                            rectSpace = new Rect(Screen.width * 0.675f, Screen.height * (1 - 0.5422f), Screen.width * 0.325f, (Screen.height * 0.5422f)/height);
                            height = 1;
                            linePosition = 0;
                        }
                        if (linePosition > (1 - height)) {
                            linePosition = (1 - height);
                        }
                        Rect rectText = new Rect(0,1 - height -linePosition, width, height);
                        GUI.DrawTextureWithTexCoords(rectSpace, yokais[aux].script, rectText, true);
                    }
                    else
                        GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.46f, Screen.width * 0.325f, Screen.height * 0.5755f), interrogation, ScaleMode.StretchToFill);


                    GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.4245f, Screen.width * 0.18f, Screen.height * 0.0333f), sideBarNotSelected, ScaleMode.StretchToFill);
                    GUI.DrawTexture(new Rect(Screen.width * (0.675f + 0.145f), Screen.height * 0.4245f, Screen.width * 0.18f, Screen.height * 0.0333f), sideBarSelected, ScaleMode.StretchToFill);
                }
                else
                {
                    GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.4245f, Screen.width * 0.325f, Screen.height * (1- 0.4245f)), scrollBackground, ScaleMode.StretchToFill);

                    if (yokais[aux].name != null && yokais[aux].status == Status.Known)
                    {
                        GUIStyle b = new GUIStyle();
                        b.alignment = TextAnchor.UpperCenter;
                        b.font = manager.font;
                        b.fontSize = 34;
                        b.fontStyle = FontStyle.Bold;
                        b.wordWrap = true;

                        float height = b.CalcHeight(new GUIContent(yokais[aux].name), Screen.width * 0.29f);
                        GUI.Label(new Rect(Screen.width * 0.69f, Screen.height * 0.48f, Screen.width * 0.31f, Screen.height * 0.04f), yokais[aux].name, b);
                        
                    }

                    if (yokais[aux].description != null && yokais[aux].status == Status.Known)
                    {
                        GUIStyle b = new GUIStyle();
                        b.alignment = TextAnchor.UpperLeft;
                        b.font = manager.font;
                        b.fontSize = 28;
                        b.wordWrap = true;                        

                        float height = b.CalcHeight(new GUIContent(yokais[aux].description), Screen.width * 0.29f);

                        if (vetorTeste.y < 0f)
                        {
                            vetorTeste.y = 0;
                        }
                        else if (vetorTeste.y > height)
                        {
                            vetorTeste.y = height;
                        }


                        vetorTeste = GUI.BeginScrollView(new Rect(Screen.width * 0.69f, Screen.height * 0.5622f, Screen.width * 0.31f, Screen.height * 0.4f), vetorTeste, new Rect(0, 0, Screen.width * 0.295f, height));
                            GUI.Label(new Rect(0, 0, Screen.width * 0.29f,height), yokais[aux].description, b);
                        GUI.EndScrollView();

                    }
                    

                    GUI.DrawTexture(new Rect(Screen.width * (0.675f + 0.145f), Screen.height * 0.4245f, Screen.width * 0.18f, Screen.height * 0.0333f), sideBarNotSelected, ScaleMode.StretchToFill);
                    GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.4245f, Screen.width * 0.18f, Screen.height * 0.0333f), sideBarSelected, ScaleMode.StretchToFill);
                }
                GUIStyle a = new GUIStyle();
                a.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(Screen.width * (0.675f), Screen.height * 0.4245f, Screen.width * 0.18f, Screen.height * 0.0333f), label1, a);
                GUI.Label(new Rect(Screen.width * (0.675f + 0.145f), Screen.height * 0.4245f, Screen.width * 0.18f, Screen.height * 0.0333f), label2, a);

            }
            else if (lastZoomCommand == 0)
            {
                GUI.DrawTexture(new Rect(Screen.width*0.675f, 0,Screen.width*0.325f,Screen.height*0.3912f),bosses[selected / 9].picture,ScaleMode.StretchToFill);
                DrawFolks(selected / 9);
            }
        
        }
    }

    public void ObtainNewYokai(int yokaiNumber)
    {
        if ((yokaiNumber >= yokais.Length) || (yokaiNumber < 0))
        {
            //print((("Yokai Desconhecido");
            return;
        }

        if (yokais[yokaiNumber].status == Status.Unknown)
            yokais[yokaiNumber].status = Status.Found;
    }

    public void DrawZoomed(Rect start, Rect end, Texture2D drawing)
    {
        float size = start.width + lastZoomCommand * (end.width - start.width);
        Vector2 position = Vector2.Lerp(new Vector2(start.x + start.width / 2.0f, start.y + start.height / 2.0f), new Vector2(end.x + end.width / 2.0f, end.y + end.height / 2.0f), (lastZoomCommand));
        GUI.DrawTexture(new Rect(position.x - size / 2.0f, position.y - size / 2.0f, size, size), drawing);
    }

    public void ResetCount()
    {
        startCount = 0;
    }

    public void DrawFolks(int boss)
    {
        GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.4245f, Screen.width * 0.325f, Screen.height * 0.5755f), bosses[boss].folks.backgroundScalable, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.4245f, Screen.width * 0.325f, Screen.height * 0.5755f), bosses[boss].folks.backgroundFixed, ScaleMode.ScaleToFit);
        foreach (YokaiGroupImage yokai in bosses[boss].folks.images) {
            if (yokais[yokai.rank].status == Status.Known)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.4245f, Screen.width * 0.325f, Screen.height * 0.5755f), yokai.imagem, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.675f, Screen.height * 0.4245f, Screen.width * 0.325f, Screen.height * 0.5755f), yokai.black, ScaleMode.ScaleToFit);
            }
        }
    }

}
