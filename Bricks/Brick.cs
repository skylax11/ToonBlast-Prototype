using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int Row;
    public int Column;
    public BrickType Type;
    public BrickInfo BrickInfo;
    public BrickGroup InvolvedGroup;

    public bool HasTransformedEver = false;

    private BrickAnimator _brickAnimator;
    private void Start()
    {
        _brickAnimator = GetComponent<BrickAnimator>();
    }
    public void Click()
    {
        if (InvolvedGroup.BrickList.Count < 2)
        {
            _brickAnimator.SingleBrick();
            return;
        }

        InvolvedGroup.CollapseAll();
    }
    public void TransformAnotherBrick()
    {
        _brickAnimator.HandleTransformAnim();
    }
    /// <summary>
    /// A transform function which will be called at end of the "Transform" animation.
    /// </summary>
    public void Transform()
    {
        GameManager _gm = GameManager.Instance;

        // if random.range is even number then choose your neighbour as (row+1,column). brick, else (row,column+1)

        Brick neighbourBrick = (UnityEngine.Random.Range(0, 2) % 2 == 0) ? 
            _gm.AllBricks[Row + 1, Column]
          : _gm.AllBricks[Row, Column + 1];

        GameManager.Instance.SetCubeInfos(gameObject, Row, Column,
            (int)Enum.ToObject(typeof(BrickType), neighbourBrick.BrickInfo.BrickType));
    }
    public void Collapse()
    {
        gameObject.SetActive(false);
        TouchSystem.Instance.LetTouch = false;
        GameManager.CollapsedBricks.Push(this);
    }
}
public class BrickGroup
{
    public List<Brick> BrickList = new List<Brick>();

    public void CollapseAll()
    {
        foreach(var item in BrickList)
            item.Collapse();

        GameManager.Instance.HandleCollapsingBricks(this);

        BrickList.Clear(); // preventing memory leak
    }

}

public enum BrickType
{
    Blue,
    Red,
    Pink,
    Purple,
    Green,
    Yellow
}