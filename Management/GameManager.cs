using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonobehaviourSingleton<GameManager>
{
    [Header("Map & Game Infos")]
    [SerializeField] int _totalRows;
    [SerializeField] int _totalCols;
    [SerializeField] int _firstGroupCapacity;
    [SerializeField] int _secondGroupCapacity;
    [SerializeField] int _thirdGroupCapacity;


    [Header("Scriptable Objects")]
    public List<BrickInfo> BrickInfos;


    [Header("Prefab to instantiate")]
    [SerializeField] GameObject _brickToSpawn;
    [SerializeField] Transform _parent;
    private Vector3 _pivot = new Vector3(-2f, -1.8f, 0);


    [Header("Brick Struct")]
    private Brick[,] _allBricks;
    [SerializeField] float _brickRelocateDuration;


    public static Stack<Brick> CollapsedBricks;

    private void Start()
    {
        CollapsedBricks = new Stack<Brick>();
        _allBricks = new Brick[_totalRows,_totalCols];

        GenerateBricks();
        GroupBricks();
    }
    int c;
    private void GenerateBricks()
    {
        for (int i = 0; i < _totalRows; i++)
        {
            for (int j = 0; j < _totalCols; j++)
            {
                GameObject spawnedCube = Instantiate(_brickToSpawn, _parent);
                spawnedCube.name = "Cube" +(c);
                c++;
                SetCubeInfos(spawnedCube,i,j);

                spawnedCube.transform.position = _pivot;
                _pivot.x += 0.5f;
            }
            _pivot.x = -2f;
            _pivot.y += 0.5f;

        }
    }
    #region GROUPING BRICKS & FLOODFILL ALGORITHM
    private void GroupBricks()
    {
        bool[,] visited = new bool[_totalRows,_totalCols];

        for (int row = 0; row < _totalRows; row++)
        {
            for(int col  = 0; col < _totalCols; col++)
            {
                if (visited[row, col] == false)
                {
                    BrickGroup group = new BrickGroup();

                    BrickType type = _allBricks[row,col].Type;

                    FloodFill(row, col, type, visited, group);

                    int initialGroupCount = group.BrickList.Count;

                    if (initialGroupCount < 2) // it is already setted as default sprite
                        continue;

                    Sprite spriteToChange = GetSprite(_allBricks[row,col].BrickInfo, initialGroupCount);

                    for (int i = 0; i < initialGroupCount; i++)
                        group.BrickList[i].GetComponent<SpriteRenderer>().sprite = spriteToChange;
                }
            }
        }

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
    #endregion
    #region COLLAPSING BRICKS , RELOCATING AND REGENERATION
    public void HandleCollapsingBricks(BrickGroup group)
    {
        List<Brick> sortedListByColumn = group.BrickList.OrderBy(x => x.Column).ToList();


        int firstColumn = sortedListByColumn[0].Column;
        int lastColumn = sortedListByColumn[sortedListByColumn.Count - 1].Column;

        RelocateBricks(firstColumn,lastColumn,group);

        GroupBricks(); 

    }
    private void RelocateBricks(int firstColumn, int lastColumn, BrickGroup group)
    {

        List<List<Brick>> gapList  = GetGapInfos(group, firstColumn, lastColumn)[0] as List<List<Brick>>;

        List<List<int>> gapRowList = GetGapInfos(group, firstColumn, lastColumn)[1] as List<List<int>>;

        int gapListCounter = 0;

        for (int column = firstColumn; column <= lastColumn; column++)
        {
            bool[] availableRows = new bool[_totalRows];

            List<Brick> gaps = gapList[gapListCounter];
            List<int> gapRowIndexes = gapRowList[gapListCounter];

            gapListCounter++;

            foreach (var item in gapRowIndexes)
                availableRows[item] = true;

            for (int row = gapRowIndexes[0] + 1; row < _totalRows; row++)
            {

                if (availableRows[row])
                    continue;

                int gapCount = CalcGapCount(gapRowIndexes,row);

                Brick currentBrick = _allBricks[row, column];

                Vector3 positionInfo = currentBrick.transform.position;

                Vector3 newPosition = new Vector3(positionInfo.x, positionInfo.y - gapCount * 0.5f, positionInfo.z);
                currentBrick.transform.DOMove(newPosition, _brickRelocateDuration);
                currentBrick.Row -= gapCount;

                _allBricks[row - gapCount, column] = currentBrick;
                _allBricks[row, column] = null;

                availableRows[row] = true;
                availableRows[row - gapCount] = false;
            }


            ReGenerateBricks(availableRows, _allBricks, gaps.Count, column);

        }
    }
    private int CalcGapCount(List<int> gapRowIndexes , int row)
    {
        int counter = 0;

        for(int i = 0; i < gapRowIndexes.Count; i++)
            if (gapRowIndexes[i] < row)
                counter++;

        return counter; 
    }
    /// <summary>
    /// Object array returns a gap list and their indexes as rows. It prevents data conflicting with Object Pooling.(Rows of the bricks also adjusted in ObjectPooling.cs)
    /// </summary>
    /// <param name="group">The brick group which is clicked on</param>
    /// <param name="firstColumn">First column of the group</param>
    /// <param name="lastColumn">Last column of the group</param>
    /// <returns></returns>
    private object[] GetGapInfos(BrickGroup group,int firstColumn , int lastColumn)
    {

        List<List<Brick>> GapList = new List<List<Brick>>();
        List<List<int>> gapRowList = new List<List<int>>();


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

            gapRowList.Add(newRowList);
        }

        return new object[]{ GapList , gapRowList };
    }
    /// <summary>
    /// Regenerates the collapsed bricks. Goals to gain performance with performing Object Pooling pattern.
    /// </summary>
    /// <param name="gaps"> The gap boolean array</param>
    /// <param name="_allBricks">All brick list.</param>
    /// <param name="gapCount">Total gap count for the current column</param>
    /// <param name="column">The current column</param>
    private void ReGenerateBricks(bool[] gaps,Brick[,] _allBricks,int gapCount,int column) => ObjectPooling.RegenerateBricks(gaps, _allBricks, CollapsedBricks,gapCount,column);

    #endregion

    public void SetCubeInfos(GameObject spawnedCube, int row, int column, bool mode = false)
    {
        int randomNumber = UnityEngine.Random.Range(0, 6);

        BrickInfo selectedType = BrickInfos[randomNumber];

        Brick spawnedBrick = null;

        if (spawnedCube.TryGetComponent(out Brick brick))
            spawnedBrick = brick;
        else
            spawnedBrick = spawnedCube.AddComponent<Brick>();


        spawnedBrick.BrickInfo = selectedType;
        spawnedBrick.Type = selectedType.BrickType;
        spawnedBrick.Row = row;
        spawnedBrick.Column = column;

        SpriteRenderer spriteRenderer = spawnedCube.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = selectedType.SpriteForms[0];
        spriteRenderer.sortingOrder = row;

        _allBricks[row, column] = spawnedBrick;
    }
}
