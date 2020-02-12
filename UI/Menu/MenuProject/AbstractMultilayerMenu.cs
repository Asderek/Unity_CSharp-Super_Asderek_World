using UnityEngine;

[System.Serializable]
public abstract class AbstractMultilayerMenu : AbstractMenu
{
    [HideInInspector]
    public LimitedIndex rowIndex;
    public LimitedIndex columnIndex;

    protected int rowUpdateDelay;
    protected int rowQtdUpdates;

    public abstract void DrawElement(Rect rect, int rowIndex, int columnIndex, bool isActive);

    public abstract float GetAxisVertical();
    public abstract float GetAxisHorizontal();


    public virtual void Init(LimitedIndex rowIndex, LimitedIndex columnIndex)
    {
        base.Init(columnIndex);
        this.rowIndex = rowIndex;
        this.columnIndex = columnIndex;

        rowUpdateDelay = 15;
        rowQtdUpdates = 0;
    }

    /// <summary>
    /// Function to make to control of movement and resizing operations also to change the selected element
    /// Should be called in Update() of Monobehavior always that menu will be displayed
    /// </summary>
    public override void Update()
    {
        base.Update();
        float axis = GetAxisVertical();

        if (axis == 0)
        {
            rowQtdUpdates = rowUpdateDelay;
            return;
        }

        if (rowQtdUpdates < rowUpdateDelay)
        {
            rowQtdUpdates++;
            return;
        }

        if (axis < 0)
        {
            rowIndex++;
            ChangeSelection();
            rowQtdUpdates = 0;
        }
        else if (axis > 0)
        {
            rowIndex--;
            ChangeSelection();
            rowQtdUpdates = 0;
        }
    }

    /// <summary>
    /// Function to calcule the position of each visible element and call DrawElement
    /// Should be call in OnGui when the menu has to be draw
    /// </summary>
    /// <see cref="DrawElement(Rect, int, bool)"/>
    public override void Draw()
    {
        DrawContext();

        if (menuIndex == null)
            return;

        if (menuIndex.Max() == 0)
            return;

        Rect menuRect = menuArea.GetRect();

        menuRect.height /= ((float)rowIndex.Max());

        for (int row = 0; row < rowIndex.Max(); row++, menuRect.y += menuRect.height)
        {
            Rect rect = new Rect(menuRect);
            rect.width = rect.width / ((float)menuIndex.Max());

            for (int column = 0; column < menuIndex.Max(); column++)
            {
                DrawElement(rect, row, column, (rowIndex.Get() == row) && (menuIndex.Get() == column));
                rect.x += rect.width;
            }
        }

    }

    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
        throw new System.NotImplementedException();
    }

    public override float GetAxis()
    {
        return GetAxisHorizontal();
    }

    public override Area GetArea(int element)
    {
        return GetArea( (int) Mathf.Floor( (float)element/(float)columnIndex.Max()), element % columnIndex.Max());
    }

    public Area GetArea(int row, int column)
    {
        if (row * columnIndex.Max() + column >= rowIndex.Max()* columnIndex.Max())
            return new Area(0, 0, 1, 1);

        Rect rect = menuArea.GetRect();
        rect.height /= ((float)rowIndex.Max());
        rect.y += row * rect.height;
        rect.width = rect.width / ((float)menuIndex.Max());
        rect.x += column*rect.width;
        return new Area(rect);
    }

}