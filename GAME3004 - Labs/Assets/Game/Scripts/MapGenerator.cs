using UnityEngine;
using System.Collections.Generic;

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
    
    protected List<GameObject> grid = new List<GameObject>();
    bool regenerateGrid = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (startHeight != height || startWidth != width || startDepth != depth || startMin != min || startMax != max)
        {
            Reset();
            regenerateGrid = true;
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
        foreach (GameObject t in grid )
        {
            Destroy(t);
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
                        var tile = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity);
                        tile.transform.SetParent(tileParent);

                        grid.Add(tile);
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

        foreach(GameObject t in grid)
        {
            int collisionCount = 0;
            for (int i = 0; i < normalArray.Length; i++)
            {
                if (Physics.Raycast(t.transform.position, normalArray[i], t.transform.localScale.magnitude * 0.5f))
                {
                    collisionCount++;
                }
            }
            if (collisionCount > 5)
            {
                disabled.Add(t);
            }
        }

        foreach(GameObject t in disabled)
        {
            var boxCollider = t.GetComponent<BoxCollider>();
            var meshRender = t.GetComponent<MeshRenderer>();

            boxCollider.enabled = false;
            meshRender.enabled = false;
        }
    }
}
