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

    public static float AnimationDuration = 1f;

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