using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to extend the concept of a Rect. Define a region as part of a other region, with all the values as percentages.
/// Defines a region that occupies part of an area (width and height) and centered on the position (x and y)
/// </summary>
[System.Serializable]
public class Area
{

    #region Structures and Properties

    /// <summary>
    /// Struct to represent a point in terms of a screen ratio
    /// </summary>
    [System.Serializable]
    public struct Position
    {
        [Tooltip("Horizontal position relative a screen or a Area")]
        [Range(0, 1f)]
        public float x;
        [Tooltip("Vertical position relative a screen or a Area")]
        [Range(0, 1f)]
        public float y;

        public Position(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Show Position (X/Y)
        /// </summary>
        public override string ToString()
        {
            return "Position (X/Y): " + x + "/" + y;
        }

    }


    /// <summary>
    /// Struct to represent a size in terms of a screen ratio
    /// </summary>
    [System.Serializable]
    public struct Size
    {
        [Tooltip("Width relative a screen or a Area")]
        [Range(0, 1f)]
        public float width;
        [Tooltip("Height relative a screen or a Area")]
        [Range(0, 1f)]
        public float height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Show size (width/height)
        /// </summary>
        public override string ToString()
        {
            return "Size (width/height): " + width + "/" + height;
        }
    }

    [Tooltip("Position relative (X,Y) of the image center")]
    public Position centerPosition;
    [Tooltip("Size relative (X,Y) of the image center")]
    public Size size;

    [Tooltip("Control velocity of growing. All process will go from zero to one adding growStep by growStep")]
    [Range(0, 1f)]
    public float growStep = 0.05f;
    [Tooltip("Control velocity of growing. All process will go from zero to one adding growStep by growStep")]
    [Range(0, 1f)]
    public float moveStep = 0.05f;

    protected enum UpdateCondition
    {
        STOPPED,
        COMING,
        GOING
    }

    private LimitedIndex percentSizeWidth;
    private LimitedIndex percentSizeHeigth;
    private UpdateCondition sizeConditionWidth = UpdateCondition.STOPPED;
    private UpdateCondition sizeConditionHeight = UpdateCondition.STOPPED;

    private Position otherPosition;
    private LimitedIndex percentMove;
    private UpdateCondition moveCondition = UpdateCondition.STOPPED;

    public delegate void Callback();
    private Callback moveCallback;
    private Callback sizeCallback;
    private Area parent;

    #endregion

    #region Constructors and Configurations

    /// <summary>
    /// Class to extend the concept of a Rect. Define a region as part of a other region, with all the values as percentages.
    /// Defines a region that occupies part of an area (width and height) and centered on the position (x and y).
    /// </summary>
    /// <param name="x">Horizontal relatives position of the center</param>
    /// <param name="y">Vertical relatives position of the center</param>
    /// <param name="width">Relative horizontal size</param>
    /// <param name="heigth">Relative vertical size</param>
    /// <param name="growStep">Increase step for change in sizes</param>
    /// <param name="moveStep">Increase step for change in moves</param>
    public Area(float x, float y, float width, float heigth, float growStep = 0.05f, float moveStep = 0.05f)
    {
        this.centerPosition.x = x;
        this.centerPosition.y = y;
        this.size.width = width;
        this.size.height = heigth;
        this.growStep = growStep;
        this.moveStep = moveStep;
        percentSizeWidth = new LimitedIndex(10000);
        percentSizeWidth.Set(percentSizeWidth.Max() - 1);
        percentSizeHeigth = new LimitedIndex(10000);
        percentSizeHeigth.Set(percentSizeWidth.Max() - 1);
        percentMove = new LimitedIndex(10000);
        percentMove.Set(percentMove.Max() - 1);
        parent = null;
    }

    /// <summary>
    /// Class to extend the concept of a Rect. Define a region as part of a other region, with all the values as percentages.
    /// </summary>
    public Area() : this(0.5f, 0.5f, 1f, 1f) { }

    /// <summary>
    /// Class to extend the concept of a Rect. Define a region as part of a other region, with all the values as percentages.
    /// Defines a region that occupies part of an area (width and height) and centered on the position (x and y)
    /// </summary>
    public Area(Area origin) : this(origin.centerPosition.x, origin.centerPosition.y, origin.size.width, origin.size.height) { }

    /// <summary>
    /// Class to extend the concept of a Rect. Define a region as part of a other region, with all the values as percentages.
    /// Defines a region that occupies part of an area (width and height) and centered on the position (x and y)
    /// </summary>
    public Area(Rect rect) : this(  ((float)(rect.x + rect.width / 2)) / ((float)Screen.width),
                                    ((float)(rect.y + rect.height / 2)) / ((float) Screen.height),
                                    ((float)(rect.width)) / ((float) Screen.width),
                                    ((float)(rect.height)) / ((float) Screen.height)) { }

    public Area(Area.Position position, Area.Size size) : this(position.x, position.y, size.width,size.height) {}

    public void CopyValues(Area template)
    {
        centerPosition = template.centerPosition;
        size = template.size;
        growStep = template.growStep;
        moveStep = template.moveStep;
        parent = template.parent;

        percentSizeHeigth.Set(percentSizeWidth.Max() - 1);
        percentSizeWidth.Set(percentSizeWidth.Max() - 1);

        sizeConditionWidth = UpdateCondition.STOPPED;
        sizeConditionHeight = UpdateCondition.STOPPED;
        moveCondition = UpdateCondition.STOPPED;
    }

    /// <summary>
    /// Register an area that should be called to calculate the Rect implicitly
    /// </summary>
    /// <param name="area">Area parent</param>
    public void SetParent(Area area)
    {
        parent = area;
    }

    /// <summary>
    /// Remove any area considered as parent
    /// </summary>
    public void RemoveParent()
    {
        parent = null;
    }

    /// <summary>
    /// Undock an area keeping size and position
    /// </summary>
    public void UndockFromParent()
    {
        Rect rect = GetRect();
        parent = null;
        centerPosition.x = ((float)(rect.x + rect.width / 2)) / ((float)Screen.width);
        centerPosition.y = ((float)(rect.y + rect.height / 2)) / ((float)Screen.height);
        size.width = ((float)(rect.width)) / ((float)Screen.width);
        size.height = ((float)(rect.height)) / ((float)Screen.height);
    }

    /// <summary>
    /// Return the parent of that Area
    /// </summary>
    /// <returns></returns>
    public Area GetParent()
    {
        return parent;
    }

    /// <summary>
    /// Show Area and Position
    /// </summary>
    public override string ToString()
    {
        return centerPosition + "\t" + size;
    }

    public override bool Equals(object other)
    {
        Area otherArea = other as Area;
        if (other == null || otherArea == null)
            return false;

        if (centerPosition.x != otherArea.centerPosition.x)
            return false;
        if (centerPosition.y != otherArea.centerPosition.y)
            return false;
        if (size.width != otherArea.size.width)
            return false;
        if (size.height != otherArea.size.height)
            return false;
        if (growStep != otherArea.growStep)
            return false;
        if (moveStep != otherArea.moveStep)
            return false;

        return true;
    }

    #endregion

    #region To Draw Functions

    /// <summary>
    /// Returns the ratio of the width that should be drawn
    /// </summary>
    public float GetPercentSizeWidth()
    {
        return (percentSizeWidth.Get() / ((float)percentSizeWidth.Max() - 1));
    }

    /// <summary>
    /// Returns the ratio of the heigth that should be drawn
    /// </summary>
    public float GetPercentSize()
    {
        return Mathf.Min(GetPercentSizeHeigth(), GetPercentSizeWidth()) * ( (parent != null) ? parent.GetPercentSize() : 1);
    }

    /// <summary>
    /// Returns the ratio of the heigth that should be drawn
    /// </summary>
    public float GetPercentSizeHeigth()
    {
        return (percentSizeHeigth.Get() / ((float)percentSizeHeigth.Max() - 1));
    }

    /// <summary>
    /// Returns the ratio of the movement that already is executed
    /// </summary>
    public float GetPercentMove()
    {
        return (percentMove.Get() / ((float)percentMove.Max() - 1));
    }

    /// <summary>
    /// Returns the ratio of the movement that already is executed
    /// </summary>
    public Rect GetRect(Rect outside)
    {
        Rect rect = new Rect(outside.x + (centerPosition.x * GetPercentMove() + otherPosition.x * (1f - GetPercentMove())) * outside.width,
                                outside.y + (centerPosition.y * GetPercentMove() + otherPosition.y * (1f - GetPercentMove())) * outside.height,
                                size.width * outside.width * GetPercentSizeWidth(),
                                size.height * outside.height * GetPercentSizeHeigth());
        rect.x -= rect.width / 2f;
        rect.y -= rect.height / 2f;
        return rect;
    }

    /// <summary>
    /// Returns the Rect to draw that Area inside all Screen
    /// </summary>
    public Rect GetRect()
    {
        if (parent == null)
            return GetRect(new Rect(0, 0, Screen.width, Screen.height));

        return GetRect(parent.GetRect());
    }

    /// <summary>
    /// Returns the Rect to draw that Area inside of the given Rect
    /// </summary>
    /// <param name="outside">Rect that contains this Area</param>
    public Vector2 GetPos(Rect outside)
    {
        Vector2 vector = new Vector2(outside.x + (centerPosition.x * GetPercentMove() + otherPosition.x * (1f - GetPercentMove())) * outside.width,
                                outside.y + (centerPosition.y * GetPercentMove() + otherPosition.y * (1f - GetPercentMove())) * outside.height);
        return vector;
    }

    /// <summary>
    /// Returns the Rect to draw that Area inside of the screen
    /// </summary>
    public Vector2 GetPos()
    {
        return GetPos(new Rect(0, 0, Screen.width, Screen.height));
    }

    #endregion

    #region To Move and Resize Functions

    /// <summary>
    /// Must be called if the area should move or resize
    /// </summary>
    public void Update()
    {
        switch (sizeConditionWidth)
        {
            case UpdateCondition.STOPPED:
                break;

            case UpdateCondition.GOING:
                percentSizeWidth.Update((int)(-growStep * percentSizeWidth.Max()));
                if (percentSizeWidth.Get() == 0)
                {
                    sizeConditionWidth = UpdateCondition.STOPPED;
                    sizeCallback();
                }
                break;

            case UpdateCondition.COMING:
                percentSizeWidth.Update((int)(growStep * percentSizeWidth.Max()));
                if (percentSizeWidth.Get() == percentSizeWidth.Max() - 1)
                {
                    sizeConditionWidth = UpdateCondition.STOPPED;
                    sizeCallback();
                }
                break;
        }

        switch (sizeConditionHeight)
        {
            case UpdateCondition.STOPPED:
                break;

            case UpdateCondition.GOING:
                percentSizeHeigth.Update((int)(-growStep * percentSizeHeigth.Max()));
                if (percentSizeHeigth.Get() == 0)
                {
                    sizeConditionHeight = UpdateCondition.STOPPED;
                    if (sizeConditionWidth == UpdateCondition.STOPPED)
                        sizeCallback();
                }
                break;

            case UpdateCondition.COMING:
                percentSizeHeigth.Update((int)(growStep * percentSizeHeigth.Max()));
                if (percentSizeHeigth.Get() == percentSizeHeigth.Max() - 1)
                {
                    sizeConditionHeight = UpdateCondition.STOPPED;
                    if (sizeConditionWidth == UpdateCondition.STOPPED)
                        sizeCallback();
                }
                break;
        }

        switch (moveCondition)
        {
            case UpdateCondition.STOPPED:
                break;

            case UpdateCondition.GOING:
                percentMove.Update((int)(-moveStep * percentMove.Max()));
                if (percentMove.Get() == 0)
                {
                    moveCondition = UpdateCondition.STOPPED;
                    moveCallback();
                }
                break;

            case UpdateCondition.COMING:
                percentMove.Update((int)(moveStep * percentMove.Max()));
                if (percentMove.Get() == percentMove.Max() - 1)
                {
                    moveCondition = UpdateCondition.STOPPED;
                    moveCallback();
                }
                break;
        }

    }

    /// <summary>
    /// Start a process to reduce the area growStep by growStep.
    /// </summary>
    /// <param name="width">True if width should reduce</param>
    /// <param name="heigth">True if heigth should reduce</param>
    /// <param name="callback">Lambda function that will be called when the process is over</param>
    /// <param name="force">When true force the current size to 100%</param>
    public void Reduce(bool width, bool heigth, Callback callback, bool force = false)
    {
        sizeConditionWidth = (width) ? UpdateCondition.GOING : UpdateCondition.STOPPED;
        sizeConditionHeight = (heigth) ? UpdateCondition.GOING : UpdateCondition.STOPPED;
        sizeCallback = callback;
        if (force)
        {
            if (width)
                percentSizeWidth.Set(percentSizeWidth.Max() - 1);
            if(heigth)
                percentSizeHeigth.Set(percentSizeHeigth.Max() - 1);
        }
    }

    /// <summary>
    /// Start a process to reduce the area growStep by growStep.
    /// </summary>
    /// <param name="width">True if width should reduce</param>
    /// <param name="heigth">True if heigth should reduce</param>
    /// <param name="force">When true force the current size to 100%</param>
    public void Reduce(bool width, bool heigth, bool force = false)
    {
        Reduce(width, heigth, () => { }, force);
    }

    /// <summary>
    /// Start a process to reduce the area growStep by growStep.
    /// </summary>
    /// <param name="callback">Lambda function that will be called when the process is over</param>
    /// <param name="force">When true force the current size to 100%</param>
    public void Reduce(Callback callback, bool force = false)
    {
        Reduce(true, true, callback, force);
    }

    /// <summary>
    /// Start a process to reduce the area growStep by growStep.
    /// </summary>
    /// <param name="force">When true force the current size to 100%</param>
    public void Reduce(bool force = false)
    {
        Reduce(true, true, () => { }, force);
    }

    /// <summary>
    /// Start a process to reduce the area growStep by growStep.
    /// </summary>
    /// <param name="width">True if width should grow</param>
    /// <param name="heigth">True if heigth should grow</param>
    /// <param name="callback">Lambda function that will be called when the process is over</param>
    /// <param name="force">When true force the current size to 100%</param>
    public void Grow(bool width, bool heigth, Callback callback, bool force = false)
    {
        sizeConditionWidth = (width) ? UpdateCondition.COMING : UpdateCondition.STOPPED;
        sizeConditionHeight = (heigth) ? UpdateCondition.COMING : UpdateCondition.STOPPED;
        sizeCallback = callback;
        if (force)
        {
            if (width)
                percentSizeWidth.Set(0);
            if (heigth)
                percentSizeHeigth.Set(0);
        }
    }

    /// <summary>
    /// Start a process to reduce the area growStep by growStep.
    /// </summary>
    /// <param name="width">True if width should grow</param>
    /// <param name="heigth">True if heigth should grow</param>
    /// <param name="force">When true force the current size to 100%</param>
    public void Grow(bool width, bool heigth, bool force = false)
    {
        Grow(width, heigth, () => { }, force);
    }

    /// <summary>
    /// Start a process to grow the area growStep by growStep.
    /// </summary>
    /// <param name="callback">Lambda function that will be called when the process is over</param>
    /// <param name="force">When true force the current size to 0%</param>
    public void Grow(Callback callback, bool force = false)
    {
        Grow(true, true, callback, force);
    }

    /// <summary>
    /// Start a process to grow the area growStep by growStep.
    /// </summary>
    /// <param name="force">When true force the current size to 0%</param>
    public void Grow(bool force = false)
    {
        Grow(true,true,() => { }, force);
    }

    /// <summary>
    /// Start a process to move from the (x,y) to given position. The move is made moveStep by moveStep.
    /// </summary>
    /// <param name="position">Destiny position</param>
    /// <param name="callback">Lambda function that will be called when the process is over</param>
    /// <param name="force">When true force the initial position to (x,y)</param>
    public void GoTo(Position position, Callback callback, bool force = true)
    {
        this.otherPosition = position;
        moveCallback = callback;
        moveCondition = UpdateCondition.GOING;
        if (force)
        {
            percentMove.Set(percentMove.Max() - 1);
        }
    }

    /// <summary>
    /// Start a process to move from the (x,y) to given position. The move is made moveStep by moveStep.
    /// </summary>
    /// <param name="position">Destiny position</param>
    /// <param name="force">When true force the initial position to (x,y)</param>
    public void GoTo(Position position, bool force = true)
    {
        GoTo(position, () => { }, force);
    }

    /// <summary>
    /// Start a process to move from the given position to (x,y). The move is made moveStep by moveStep.
    /// </summary>
    /// <param name="position">Origin position</param>
    /// <param name="callback">Lambda function that will be called when the process is over</param>
    /// <param name="force">When true force the initial position to origin</param>
    public void ComeFrom(Position position, Callback callback, bool force = true)
    {
        this.otherPosition = position;
        moveCallback = callback;
        moveCondition = UpdateCondition.COMING;
        if (force)
        {
            percentMove.Set(0);
        }
    }

    /// <summary>
    /// Start a process to move from the given position to (x,y). The move is made moveStep by moveStep.
    /// </summary>
    /// <param name="position">Origin position</param>
    /// <param name="force">When true force the initial position to origin</param>
    public void ComeFrom(Position position, bool force = true)
    {
        ComeFrom(position, () => { }, force);
    }

    #endregion

    #region Aditional Functions

    public void AdjustToSquare ()
    {
        if (size.width * Screen.width == size.height * Screen.height)
            return;

        if (size.width * Screen.width > size.height * Screen.height)
        {
            size.width = (size.height * Screen.height) / ((float)Screen.width);
        }
        else if (size.width * Screen.width < size.height * Screen.height)
        {
            size.height = (size.width * Screen.width) / ((float)Screen.height);
        }
    }

    #endregion
}
