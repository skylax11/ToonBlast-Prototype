using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonobehaviourSingleton<GameManager>
{
    [Header("Map & Game Infos")]
    public int TotalRows;
    public int TotalCols;
    [SerializeField] int _firstGroupCapacity;
    [SerializeField] int _secondGroupCapacity;
    [SerializeField] int _thirdGroupCapacity;


    [Header("Scriptable Objects")]
    public List<BrickInfo> BrickInfos;


    [Header("Prefab to instantiate")]
    [SerializeField] GameObject _brickToSpawn;
    [SerializeField] Transform _parent;
    private Vector3 _pivot = new Vector3(-2f, -1.8f, 0);

    // Min x and Max Y values for regenerating bricks.
    public static float GridMinX = -2f;
    public static float GridMinY = -1.8f;

    public static float GridMaxY = 2.2f;

    [Header("Brick Struct")]
    public Brick[,] AllBricks;

    [Header("Brick Grouper & Relocator")]
    private BrickGrouper _brickGrouper;
    private BrickRelocator _brickRelocator;

    public static Stack<Brick> CollapsedBricks;

    private void Start()
    {
        Application.targetFrameRate = 60;

        DOTween.Init();

        _brickGrouper = GetComponent<BrickGrouper>();
        _brickRelocator = GetComponent<BrickRelocator>();

        CollapsedBricks = new Stack<Brick>();
        AllBricks = new Brick[TotalRows, TotalCols];

        SetInitialValues();

        GenerateBricks();
        GroupBricks();
    }

    /// <summary>
    /// Sets the initial values for utilizing object pooling system.
    /// </summary>
    private void SetInitialValues()
    {
        ObjectPooling.GridMinX = GridMinX;
        ObjectPooling.TotalRows = TotalRows;
        ObjectPooling.GridMaxY = GridMinY + 0.5f * TotalRows;

        // sets initial values to variables for both scripts...
        _brickGrouper.BrickGrouperInitializer(AllBricks, TotalRows, TotalCols, _firstGroupCapacity, _secondGroupCapacity, _thirdGroupCapacity);
        _brickRelocator.BrickRelocatorInitializer(AllBricks,TotalRows);
    }
    private void GenerateBricks()
    {
        for (int i = 0; i < TotalRows; i++)
        {
            for (int j = 0; j < TotalCols; j++)
            {
                GameObject spawnedCube = Instantiate(_brickToSpawn, _parent);

                SetCubeInfos(spawnedCube,i,j);

                spawnedCube.transform.position = _pivot;
                _pivot.x += 0.5f;
            }
            _pivot.x = -2f;
            _pivot.y += 0.5f;

        }
    }
    #region GROUPING BRICKS & DEADLOCK SOLUTION & FLOODFILL ALGORITHM
    public void GroupBricks()
    {
        _brickGrouper.GroupBricks();
    }
    #endregion
    #region COLLAPSING BRICKS , RELOCATING AND REGENERATION
    public void HandleCollapsingBricks(BrickGroup group)
    {
        List<Brick> sortedListByColumn = group.BrickList.OrderBy(x => x.Column).ToList();


        int firstColumn = sortedListByColumn[0].Column;
        int lastColumn = sortedListByColumn[sortedListByColumn.Count - 1].Column;

        _brickRelocator.RelocateBricks(firstColumn,lastColumn,group);

    }
    #endregion

    public void SetCubeInfos(GameObject spawnedCube, int row, int column, int number = -1)
    {
        if(number == -1)
            number = UnityEngine.Random.Range(0, 6);

        BrickInfo selectedType = BrickInfos[number];

        Brick spawnedBrick = null;

        if (spawnedCube.TryGetComponent(out Brick brick))
            spawnedBrick = brick;

        spawnedBrick.BrickInfo = selectedType;
        spawnedBrick.Type = selectedType.BrickType;
        spawnedBrick.Row = row;
        spawnedBrick.Column = column;

        SpriteRenderer spriteRenderer = spawnedCube.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = selectedType.SpriteForms[0];
        spriteRenderer.sortingOrder = row;

        AllBricks[row, column] = spawnedBrick;
    }
}
