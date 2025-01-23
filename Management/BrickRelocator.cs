using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickRelocator : MonoBehaviour
{
    private Brick[,] _allBricks;
    private int _totalRows;

    public void BrickRelocatorInitializer(Brick[,] allBricks, int row)
    {
        _allBricks = allBricks;
        _totalRows = row;
    }

    public void RelocateBricks(int firstColumn, int lastColumn, BrickGroup group)
    {
        GapInfo allGapInfos = new GapInfo();

        allGapInfos.SetGapInfos(group, firstColumn, lastColumn);

        int gapListCounter = 0;

        for (int column = firstColumn; column <= lastColumn; column++)
        {
            bool[] availableRows = new bool[_totalRows];

            List<Brick> gaps = allGapInfos.GapList[gapListCounter];
            List<int> gapRowIndexes = allGapInfos.GapRowList[gapListCounter];

            gapListCounter++;

            foreach (var item in gapRowIndexes)
                availableRows[item] = true;

            for (int row = gapRowIndexes[0] + 1; row < _totalRows; row++)
            {

                if (availableRows[row])
                    continue;

                int gapCount = CalcGapCount(gapRowIndexes, row);

                Brick currentBrick = _allBricks[row, column];

                Vector3 positionInfo = currentBrick.transform.position;

                Vector3 newPosition = new Vector3(positionInfo.x, positionInfo.y - gapCount * 0.5f, positionInfo.z);
                currentBrick.transform.DOMove(newPosition, BrickAnimator.RelocateAnimationDuration);
                currentBrick.Row -= gapCount;

                _allBricks[row - gapCount, column] = currentBrick;
                _allBricks[row, column] = null;

                availableRows[row] = true;
                availableRows[row - gapCount] = false;

                currentBrick.GetComponent<SpriteRenderer>().sortingOrder = currentBrick.Row;
            }

            ReGenerateBricks(availableRows, _allBricks, gaps.Count, column);

        }

        StartCoroutine(nameof(SetLetTouch), BrickAnimator.RelocateAnimationDuration);

        allGapInfos.ClearReferences();
    }

    /// <summary>
    /// After "delay" miliseconds LetTouch variable will be setted true.
    /// </summary>
    /// <param name="seconds"></param>
    private IEnumerator SetLetTouch(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        TouchSystem.Instance.LetTouch = true;
        GameManager.Instance.GroupBricks();

    }
    private int CalcGapCount(List<int> gapRowIndexes, int row)
    {
        int counter = 0;

        for (int i = 0; i < gapRowIndexes.Count; i++)
            if (gapRowIndexes[i] < row)
                counter++;

        return counter;
    }
    /// <summary>
    /// Regenerates the collapsed bricks. Goals to gain performance with performing Object Pooling pattern.
    /// </summary>
    /// <param name="gaps"> The gap boolean array</param>
    /// <param name="_allBricks">All brick list.</param>
    /// <param name="gapCount">Total gap count for the current column</param>
    /// <param name="column">The current column</param>
    private void ReGenerateBricks(bool[] gaps, Brick[,] _allBricks, int gapCount, int column)
    {
        ObjectPooling.RegenerateBricks(gaps, _allBricks, GameManager.CollapsedBricks, gapCount, column);
    }

}
