using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("World Properties")]
    [Range(8,64)] public int height, width, depth;
    protected int startHeight, startWidth, startDepth;
    protected float startMin, startMax;

    [Header("Scale Values")]
    [Range(8, 64)] public float min = 16f;
    [Range(8, 64)] public float max = 24f;

    [Header("Tile Properties")]
    public GameObject tilePrefab;
    public Transform tileParent;
    
    protected Dictionary<Vector3,GameObject> grid = new Dictionary<Vector3, GameObject>();
    bool regenerateGrid = false;

    public static Action<GameObject> OnTileDestroyed;

    private void OnEnable()
    {
        OnTileDestroyed += EnableNeighbours;
    }
    private void OnDisable()
    {
        OnTileDestroyed -= EnableNeighbours;
    }

    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (startHeight != height || startWidth != width || startDepth != depth || startMin != min || startMax != max)
        {
            regenerateGrid = true;
            Reset();
        }

        if (regenerateGrid)
        {
            Initialize();
            regenerateGrid = false;
        }
    }

    void Initialize()
    {
        startHeight = height; startWidth = width; startDepth = depth;
        startMin = min; startMax = max;

        Regenerate();
        DisableNonVisibleTiles();
    }

    private void Reset()
    {
        foreach (var t in grid )
        {
            Destroy(t.Value);
        }
        
        grid.Clear();
    }

    void Regenerate()
    {
        float ranScale = Random.Range(min, max);
        float offsetX = Random.Range(-1024f, 1024f);
        float offsetY = Random.Range(-1024f, 1024f);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    var perlinNoise = Mathf.PerlinNoise((x + offsetX) / ranScale
                        , (z + offsetY) / ranScale) * depth * 0.5f;

                    if (y< perlinNoise)
                    {
                        Vector3 newPos = new Vector3 (x, y, z);
                        
                        var tile = Instantiate(tilePrefab, newPos, Quaternion.identity);
                        tile.transform.SetParent(tileParent);

                        tile.GetComponent<TileProperties>().Initialize(newPos);

                        grid[newPos] = tile;
                    }
                }
            }
        }
    }

    void DisableNonVisibleTiles()
    {
        // disable the colliders and rendering of tiles with faces not shown
        var normalArray = new Vector3[] { Vector3.up, Vector3.down, Vector3.forward,
        Vector3.back, Vector3.right, Vector3.left};
        List<GameObject> disabled = new List<GameObject>();

        foreach(var t in grid)
        {
            if (!IsTileExposed(t.Value))
            {
                disabled.Add(t.Value);
            }
        }

        foreach(GameObject t in disabled)
        {
            t.GetComponent<TileProperties>().ModifyVisuals(false);
        }
    }

    void EnableNeighbours(GameObject destroyedTile)
    {
        if (regenerateGrid) { return; }

        grid.Remove(destroyedTile.transform.position);
        
        var normalArray = new Vector3[] { Vector3.up, Vector3.down, Vector3.forward,
        Vector3.back, Vector3.right, Vector3.left};


    }

    bool IsTileExposed(GameObject tile)
    {
        var normalArray = new Vector3[] { Vector3.up, Vector3.down, Vector3.forward,
        Vector3.back, Vector3.right, Vector3.left};

        if (!grid.ContainsValue(tile)) { return false; }

        foreach(var n in normalArray)
        {
            if (!grid.ContainsKey(tile.transform.position + n))
            {
                return true;
            }
        }

        return false;
    }
}
