using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ObjectPooling
{

    public static float GridMinX;
    public static float GridMaxY;

    public static void RegenerateBricks(bool[] gaps, Brick[,] allBricks , Stack<Brick> collapsed,int gapCount,int column)
    {
        for(int row = 0; row < 8; row++)
        {
            if (!gaps[row])
                continue;

            Brick regeneratedBrick = collapsed.Pop();

            MoveBrick(column * 0.5f, gapCount * 0.5f, regeneratedBrick);

            GameManager.Instance.SetCubeInfos(regeneratedBrick.gameObject, row, column, true);

            regeneratedBrick.transform.gameObject.SetActive(true);

            gaps[row] = false;

            gapCount--;

        }

        SetLetTouch((int)(Brick.AnimationDuration * 1000));

    }
    public static void MoveBrick(float goRight , float goDown, Brick regeneratedBrick)
    {
        regeneratedBrick.transform.position = new Vector3(regeneratedBrick.transform.position.x, GridMaxY + 1f, 0);

        Vector3 positionInfo = regeneratedBrick.transform.position;

        Vector3 newPosition = new Vector3(GridMinX + goRight , GridMaxY - goDown, positionInfo.z);

        regeneratedBrick.transform.DOMove(newPosition, Brick.AnimationDuration);
    }
    /// <summary>
    /// After "delay" miliseconds LetTouch variable will be setted true.
    /// </summary>
    /// <param name="delay"></param>
    public static async void SetLetTouch(int delay)
    {
        await Task.Delay(delay);

        TouchSystem.Instance.LetTouch = true;
        GameManager.Instance.GroupBricks();
    }
}
