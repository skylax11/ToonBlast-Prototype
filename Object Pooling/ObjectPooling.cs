using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooling
{
    public static void RegenerateBricks(bool[] gaps, Brick[,] allBricks , Stack<Brick> collapsed,int gapCount,int column)
    {
        for(int row = 0; row < 8; row++)
        {
            if (!gaps[row])
                continue;

            Brick regeneratedBrick = collapsed.Pop();

            regeneratedBrick.transform.position = new Vector3(regeneratedBrick.transform.position.x, 2.2f, 0);

            Vector3 positionInfo = regeneratedBrick.transform.position;

            MoveBrick(positionInfo, column * 0.5f, gapCount * 0.5f, regeneratedBrick);

            GameManager.Instance.SetCubeInfos(regeneratedBrick.gameObject, row, column, true);

            regeneratedBrick.transform.gameObject.SetActive(true);

            gaps[row] = false;

            gapCount--;

        }
    }
    public static void MoveBrick(Vector3 positionInfo , float goRight , float goDown, Brick regeneratedBrick)
    {
        Vector3 newPosition = new Vector3(-2 + goRight , 2.2f - goDown, positionInfo.z);
        regeneratedBrick.transform.DOMove(newPosition, 1f);
    }
}
