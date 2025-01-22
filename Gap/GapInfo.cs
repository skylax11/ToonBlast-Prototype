using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GapInfo
{
    public List<List<Brick>> GapList;
    public List<List<int>> GapRowList;


    public GapInfo()
    {
        GapList = new List<List<Brick>>();
        GapRowList = new List<List<int>>();
    }
    public void ClearReferences()
    {
        GapList.Clear();
        GapRowList.Clear();
    }
    /// <summary>
    /// Object array returns a gap list and their indexes as rows. It prevents data conflicting with Object Pooling.(Rows of the bricks also adjusted in ObjectPooling.cs)
    /// </summary>
    /// <param name="group">The brick group which is clicked on</param>
    /// <param name="firstColumn">First column of the group</param>
    /// <param name="lastColumn">Last column of the group</param>
    /// <returns></returns>
    public void SetGapInfos(BrickGroup group, int firstColumn, int lastColumn)
    {
        for (int column = firstColumn; column <= lastColumn; column++)
        {
            List<Brick> gaps = group.BrickList.Where(x => x.Column == column).OrderBy(x => x.Row).ToList();
            GapList.Add(gaps);

        }
        for (int i = 0; i < GapList.Count; i++)
        {
            List<int> newRowList = new List<int>();

            for (int j = 0; j < GapList[i].Count; j++)
            {
                newRowList.Add(GapList[i][j].Row);
            }

            GapRowList.Add(newRowList);
        }
    }
}
