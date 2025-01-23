using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickGrouper : MonoBehaviour
{
    private GameManager _gameManager;

    private Brick[,] _allBricks;

    private int _totalRows;
    private int _totalCols;
    private int _firstGroupCapacity;
    private int _secondGroupCapacity;
    private int _thirdGroupCapacity;

    [Header("Deadlock Control")]
    private int _collapsableGroupCount;

    public void BrickGrouperInitializer(Brick[,] allBricks , int row , int col , int firstGroupCap , int secondGroupCap , int thirdGroupCap)
    {
        _totalRows = row;
        _totalCols = col;
        _firstGroupCapacity = firstGroupCap;
        _secondGroupCapacity = secondGroupCap;
        _thirdGroupCapacity = thirdGroupCap;
        _allBricks = allBricks;
    }

    public void GroupBricks()
    {
        _collapsableGroupCount = 0;

        bool[,] visited = new bool[_totalRows, _totalCols];

        for (int row = 0; row < _totalRows; row++)
        {
            for (int col = 0; col < _totalCols; col++)
            {
                if (visited[row, col] == false)
                {
                    BrickGroup group = new BrickGroup();

                    BrickType type = _allBricks[row, col].Type;

                    FloodFill(row, col, type, visited, group);

                    int initialGroupCount = group.BrickList.Count;

                    if (initialGroupCount >= 2)
                        _collapsableGroupCount++;

                    Sprite spriteToChange = GetSprite(_allBricks[row, col].BrickInfo, initialGroupCount);

                    for (int i = 0; i < initialGroupCount; i++)
                        group.BrickList[i].GetComponent<SpriteRenderer>().sprite = spriteToChange;
                }
            }
        }

        if (_collapsableGroupCount == 0)
            Deadlock();

    }
    private void FloodFill(int row, int col, BrickType type, bool[,] visited, BrickGroup group)
    {
        if (row < 0 || row >= _totalRows || col < 0 || col >= _totalCols)
            return;
        if (visited[row, col] || _allBricks[row, col].Type != type)
            return;

        visited[row, col] = true;

        group.BrickList.Add(_allBricks[row, col]);
        _allBricks[row, col].InvolvedGroup = group;


        FloodFill(row + 1, col, type, visited, group);
        FloodFill(row, col + 1, type, visited, group);
        FloodFill(row - 1, col, type, visited, group);
        FloodFill(row, col - 1, type, visited, group);

    }
    private void Deadlock()
    {
        Stack<Brick> tempStack = new Stack<Brick>();

        for (int i = 0; i < 3; i++)
        {
            int randomRow = UnityEngine.Random.Range(1, _totalRows - 1);
            int randomColumn = UnityEngine.Random.Range(1, _totalCols - 1);

            Brick randomlySelected = _allBricks[randomRow, randomColumn];

            if (randomlySelected.HasTransformedEver)
                continue;

            tempStack.Push(randomlySelected);

            randomlySelected.HasTransformedEver = true;

            randomlySelected.TransformAnotherBrick();

        }

        int stackCount = tempStack.Count;

        for (int i = 0; i < stackCount; i++)
            tempStack.Pop().HasTransformedEver = false;

        StartCoroutine(nameof(GroupingBricksWithDelay), BrickAnimator.TransformClipLength);

    }
    private IEnumerator GroupingBricksWithDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GroupBricks();
    }
    private Sprite GetSprite(BrickInfo info, int count)
    {
        if (count >= _firstGroupCapacity && count < _secondGroupCapacity)
            return info.SpriteForms[1];
        else if (count >= _secondGroupCapacity && count < _thirdGroupCapacity)
            return info.SpriteForms[2];
        else if (count >= _thirdGroupCapacity)
            return info.SpriteForms[3];

        return info.SpriteForms[0];
    }
}
