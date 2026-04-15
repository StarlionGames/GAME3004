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

    List<Vector3> normalArray = new List<Vector3>();

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
        normalArray.Clear();
        
        startHeight = height; startWidth = width; startDepth = depth;
        startMin = min; startMax = max;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue; 

                    normalArray.Add(new Vector3(x, y, z));
                }
            }
        }

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

                        grid[newPos] = tile;

                        TileTypes t = RandomAssignTileType(newPos, perlinNoise);
                        tile.GetComponent<TileProperties>().Initialize(newPos,t);
                    }
                }
            }
        }
    }

    void DisableNonVisibleTiles()
    {
        List<GameObject> disabled = new List<GameObject>();

        foreach(var t in grid)
        {
            if (IsTileExposed(t.Value))
            {
                AssignExposedAsGrass(t.Value);
            }
            
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
        
        foreach(var n in normalArray)
        {
            if (grid.ContainsKey(destroyedTile.transform.position + n))
            {
                TileProperties tile = grid[destroyedTile.transform.position + n].GetComponent<TileProperties>();
                
                if (tile._isVisible) { continue; }
                else
                {
                    tile.ModifyVisuals(true);
                }
            }
        }
    }

    bool IsTileExposed(GameObject tile)
    {
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

    TileTypes RandomAssignTileType(Vector3 pos, float perlin)
    {
        if (pos.y >= Mathf.FloorToInt(perlin)) return TileTypes.Grass;

        if (pos.y<perlin - 2)
        {
            int ran = Random.Range(1, 100);

            if (ran <= 3) { return TileTypes.Diamond; }
            else if (ran <= 10) { return TileTypes.Gold; }
            else if (ran <= 20) { return TileTypes.Iron; }
            else if (ran <= 25) { return TileTypes.Coal; }
        }

        return TileTypes.Stone;
    }

    void AssignExposedAsGrass(GameObject tile)
    {
        tile.GetComponent<TileProperties>().tileType = TileTypes.Grass;
        tile.GetComponent<TileProperties>().DetermineTypeVisuals();
    }
}
